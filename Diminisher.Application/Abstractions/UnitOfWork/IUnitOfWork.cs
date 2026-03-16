using Application.Abstractions.Repository;

namespace Application.Abstractions.UnitOfWork;

public interface IUnitOfWork
{
    IUrlRepository UrlRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
    public void ClearTrackedEntities();
}