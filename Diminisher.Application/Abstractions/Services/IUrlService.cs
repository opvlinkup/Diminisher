using Application.DTO;
using Domain.Models;

namespace Application.Abstractions;

public interface IUrlService
{
        Task<IReadOnlyList<UrlDto>> GetAllAsync(CancellationToken ct);

        Task<UrlDto> CreateAsync(CreateShortUrlDto request, CancellationToken ct);

        Task<string?> ResolveAsync(string code, CancellationToken ct);

        Task UpdateAsync(Guid id, UpdateLongUrlDto dto, CancellationToken ct);

        Task DeleteAsync(Guid id, CancellationToken ct);
}