using ApiTemplate.Domain.Common;
using ApiTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiTemplate.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Name).HasMaxLength(200).IsRequired();
            entity.Property(product => product.Description).HasMaxLength(1000);
            entity.HasIndex(product => product.Name);

            entity.HasData(
                new Product
                {
                    Id = Guid.Parse("8d4e2f2a-1c3b-4f5e-9a7d-2b1c3d4e5f60"),
                    Name = "Notebook Dell Inspiron 15",
                    Description = "Notebook para desenvolvimento com 16 GB de RAM e SSD de 512 GB",
                    Price = 4599.90m,
                    IsActive = true,
                    CreatedAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = Guid.Parse("b1a2c3d4-e5f6-4789-8a0b-1c2d3e4f5a6b"),
                    Name = "Mouse sem fio",
                    Description = "Mouse ergonômico com conexão Bluetooth e USB",
                    Price = 149.90m,
                    IsActive = true,
                    CreatedAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = Guid.Parse("c3d4e5f6-a7b8-4c90-8d1e-2f3a4b5c6d7e"),
                    Name = "Teclado mecânico",
                    Description = "Switches vermelhos, layout ABNT2",
                    Price = 259.90m,
                    IsActive = true,
                    CreatedAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                });
        });
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = utcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAtUtc = utcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
