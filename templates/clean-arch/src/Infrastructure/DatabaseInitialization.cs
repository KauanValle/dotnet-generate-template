using ApiTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTemplate.Infrastructure;

public static class DatabaseInitialization
{
    /// <summary>
    /// Creates the InMemory store up front so HasData seed data is applied on first request.
    /// Relational providers (e.g. SqlServer) are skipped: use migrations instead
    /// (see README - "Trocando o banco de dados").
    /// </summary>
    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<AppDbContext>().Database;

        if (database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            await database.EnsureCreatedAsync();
        }
    }
}
