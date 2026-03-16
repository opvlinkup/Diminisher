using Application.Abstractions;
using Application.Abstractions.Repository;
using Application.Abstractions.UnitOfWork;
using Infrastructure.Database;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["DB_CONNECTION"] ?? throw new InvalidOperationException("DB connection isn't set in configuration.");

            services.AddDbContextPool<DiminisherDbContext>((sp, options) =>
            {
                DbContextConfigurator.Configure(options, connectionString);
            });
            services.AddScoped<IUrlRepository, UrlRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
}