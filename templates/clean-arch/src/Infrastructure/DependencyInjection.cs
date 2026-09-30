using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Infrastructure.Data;
using ApiTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTemplate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Database:Provider"];

        services.AddDbContext<AppDbContext>(options =>
        {
            if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                var connectionString = configuration.GetConnectionString("SqlServer")
                    ?? throw new InvalidOperationException(
                        "Connection string 'SqlServer' was not found. Add it to appsettings.json or switch Database:Provider back to 'InMemory'.");

                options.UseSqlServer(connectionString);
            }
            else
            {
                // Default provider: runs anywhere with no external dependencies.
                // Switch to "SqlServer" in appsettings.json for a real database.
                options.UseInMemoryDatabase("ApiTemplateDb");
            }
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
