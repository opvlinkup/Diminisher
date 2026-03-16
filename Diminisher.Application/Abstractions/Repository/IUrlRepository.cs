using Domain.Models;

namespace Application.Abstractions.Repository;

public interface IUrlRepository
{
    Task<ShortUrl?> GetByCodeAsync(string code, CancellationToken ct);
    Task<string?> ResolveUrlAsync(string code, CancellationToken ct);
    Task<ShortUrl?> GetByHashAsync(byte[] hash, CancellationToken ct);
    Task<ShortUrl?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<ShortUrl>> GetAllAsync(CancellationToken ct);

    Task AddAsync(ShortUrl entity, CancellationToken ct);

    Task DeleteAsync(ShortUrl entity, CancellationToken ct);
    
}