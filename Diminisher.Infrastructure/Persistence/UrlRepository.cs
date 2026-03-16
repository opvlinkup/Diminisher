using System.Data;
using Application.Abstractions.Repository;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class UrlRepository(DiminisherDbContext context) : IUrlRepository
{
    public async Task<IReadOnlyList<ShortUrl>> GetAllAsync(CancellationToken ct)
    {
        return await context.Urls
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Take(100)
            .ToListAsync(ct);
    }

    public async Task<ShortUrl?> GetByCodeAsync(string code, CancellationToken ct)
    {
        return await context.Urls
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code, ct);
    }

    public async Task<ShortUrl?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Urls
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<ShortUrl?> GetByHashAsync(byte[] hash, CancellationToken ct)
    {
        return await context.Urls
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.LongUrlHash == hash, ct);
    }

    public async Task AddAsync(ShortUrl entity, CancellationToken ct)
    {
        await context.Urls.AddAsync(entity, ct);
    }

    public Task DeleteAsync(ShortUrl entity, CancellationToken ct)
    {
        context.Urls.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<string?> ResolveUrlAsync(string code, CancellationToken ct)
    {
        var url = await context.Urls
            .Where(x => x.Code == code)
            .Select(x => new { x.LongUrl })
            .FirstOrDefaultAsync(ct);

        if (url == null)
            return null;

        await context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE ShortUrls SET Clicks = Clicks + 1 WHERE Code = {code}", ct);

        return url.LongUrl;
    }
}