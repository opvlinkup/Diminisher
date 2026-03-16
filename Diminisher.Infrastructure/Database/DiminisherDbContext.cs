using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class DiminisherDbContext(DbContextOptions<DiminisherDbContext> options) : DbContext(options)
{
    public DbSet<ShortUrl> Urls => Set<ShortUrl>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(DiminisherDbContext).Assembly);
    }
}