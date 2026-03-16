using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Diminisher.Infrastructure.Database;

public sealed class DiminisherDbContextFactory : IDesignTimeDbContextFactory<DiminisherDbContext>
{
    public DiminisherDbContext CreateDbContext(string[] args)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../Diminisher.API");
        
        if (!Directory.Exists(path))
            path = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(path, "appsettings.json"), optional: false)
            .AddEnvironmentVariables()
            .Build();
        
        var connectionString = configuration["DB_CONNECTION"]
                               ?? throw new InvalidOperationException("Connection string not found.");

        var optionsBuilder = new DbContextOptionsBuilder<DiminisherDbContext>();

        DbContextConfigurator.Configure(optionsBuilder, connectionString);

        return new DiminisherDbContext(optionsBuilder.Options);
    }
}