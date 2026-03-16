using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public static class DbContextConfigurator
{
    public static void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            mysqlOptions =>
            {
                mysqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);

                mysqlOptions.CommandTimeout(30);
            });

        options.EnableDetailedErrors(false);
        options.EnableSensitiveDataLogging(false);
    }
}