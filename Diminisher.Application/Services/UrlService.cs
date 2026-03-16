using Application.Abstractions;
using Application.Abstractions.Generators;
using Application.Abstractions.UnitOfWork;
using Application.DTO;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Application.Services;

public sealed class UrlService(
    IUnitOfWork unitOfWork,
    IShortCodeGenerator generator,
    IUrlHasher hasher) : IUrlService
{
    private const int MaxCodeGenerationAttempts = 5;

    public async Task<IReadOnlyList<UrlDto>> GetAllAsync(CancellationToken ct)
    {
        var urls = await unitOfWork.UrlRepository.GetAllAsync(ct);

        return urls.Select(x => new UrlDto
        {
            Id = x.Id,
            LongUrl = x.LongUrl,
            ShortUrl = x.Code,
            CreatedAt = x.CreatedAt,
            Clicks = x.Clicks
        }).ToList();
    }

public async Task<UrlDto> CreateAsync(CreateShortUrlDto dto, CancellationToken ct)
{
    if (string.IsNullOrWhiteSpace(dto.LongUrl))
        throw new ArgumentException("URL cannot be empty");

    if (!Uri.TryCreate(dto.LongUrl, UriKind.Absolute, out var uri) ||
        (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        throw new ArgumentException("Invalid URL");

    var hash = hasher.ComputeHash(dto.LongUrl);


    var existing = await unitOfWork.UrlRepository.GetByHashAsync(hash, ct);
    if (existing != null)
        return new UrlDto
        {
            Id = existing.Id,
            LongUrl = existing.LongUrl,
            ShortUrl = existing.Code,
            CreatedAt = existing.CreatedAt,
            Clicks = existing.Clicks
        };

    for (var attempt = 0; attempt < MaxCodeGenerationAttempts; attempt++)
    {
        var entity = new ShortUrl
        {
            Id = Guid.NewGuid(),
            LongUrl = dto.LongUrl,
            LongUrlHash = hash,
            Code = generator.Generate(),
            CreatedAt = DateTime.UtcNow,
            Clicks = 0
        };

        await unitOfWork.UrlRepository.AddAsync(entity, ct);

        try
        {
            await unitOfWork.SaveChangesAsync(ct);

            return new UrlDto
            {
                Id = entity.Id,
                LongUrl = entity.LongUrl,
                ShortUrl = entity.Code,
                CreatedAt = entity.CreatedAt,
                Clicks = entity.Clicks
            };
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is MySqlException mysqlEx && mysqlEx.Number == 1062)
            {
                if (mysqlEx.Message.Contains("LongUrlHash"))
                {
                    unitOfWork.ClearTrackedEntities();
                    
                    var alreadyExistingUrl = await unitOfWork.UrlRepository.GetByHashAsync(hash, ct);
                    
                    if (alreadyExistingUrl != null)
                        return new UrlDto
                        {
                            Id = alreadyExistingUrl.Id,
                            LongUrl = alreadyExistingUrl.LongUrl,
                            ShortUrl = alreadyExistingUrl.Code,
                            CreatedAt = alreadyExistingUrl.CreatedAt,
                            Clicks = alreadyExistingUrl.Clicks
                        };
                }
                else if (mysqlEx.Message.Contains("Code"))
                {
                    unitOfWork.ClearTrackedEntities();
                    continue;
                }
            }
            
            throw;
        }
    }

    throw new InvalidOperationException("Failed to generate unique short code after multiple attempts");
}

    public async Task<string?> ResolveAsync(string code, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return await unitOfWork.UrlRepository.ResolveUrlAsync(code, ct);
    }

    public async Task UpdateAsync(Guid id, UpdateLongUrlDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.LongUrl))
            throw new ArgumentException("URL cannot be empty");

        if (!Uri.TryCreate(dto.LongUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Invalid URL");

        var entity = await unitOfWork.UrlRepository.GetByIdAsync(id, ct);

        if (entity is null)
            throw new KeyNotFoundException("URL not found");

        entity.LongUrl = dto.LongUrl;
        entity.LongUrlHash = hasher.ComputeHash(dto.LongUrl);

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await unitOfWork.UrlRepository.GetByIdAsync(id, ct);

        if (entity is null)
            return;

        await unitOfWork.UrlRepository.DeleteAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
    
    private static bool IsDuplicateLongUrl(DbUpdateException ex)
        => ex.InnerException is MySqlException mysql &&
           mysql.Number == 1062 &&
           mysql.Message.Contains("LongUrlHash");

    private static bool IsDuplicateCode(DbUpdateException ex)
        => ex.InnerException is MySqlException mysql &&
           mysql.Number == 1062 &&
           mysql.Message.Contains("Code");
}