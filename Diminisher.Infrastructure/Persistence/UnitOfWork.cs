using Application.Abstractions.Repository;
using Application.Abstractions.UnitOfWork;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class UnitOfWork(DiminisherDbContext context, IUrlRepository urlRepository) : IUnitOfWork
{
    private readonly DiminisherDbContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
    
    public IUrlRepository UrlRepository { get; } = urlRepository;


    public Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return _dbContext.SaveChangesAsync(ct);
    }
    
    public void ClearTrackedEntities()
    {
        foreach (var entry in _dbContext.ChangeTracker.Entries().ToList())
        {
            entry.State = EntityState.Detached;
        }
    }
}