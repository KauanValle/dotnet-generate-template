using ApiTemplate.Application.Common.Exceptions;
using ApiTemplate.Application.Products;
using ApiTemplate.Domain.Entities;
using ApiTemplate.Infrastructure.Data;
using ApiTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiTemplate.Tests.Products;

public sealed class ProductServiceTests
{
    private static ProductService CreateService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated(); // applies the HasData seed

        var repository = new Repository<Product>(context);

        return new ProductService(repository);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSeededProducts()
    {
        var service = CreateService();

        var products = await service.GetAllAsync();

        Assert.True(products.Count >= 3);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_PersistsProduct()
    {
        var service = CreateService();
        var request = new CreateProductRequest("Monitor 27 polegadas", "144 Hz, IPS", 1299.90m);

        var response = await service.CreateAsync(request);

        Assert.NotEqual(Guid.Empty, response.Id);
        var stored = await service.GetByIdAsync(response.Id);
        Assert.Equal("Monitor 27 polegadas", stored.Name);
        Assert.Equal(1299.90m, stored.Price);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidPrice_ThrowsValidationException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(
            () => service.CreateAsync(new CreateProductRequest("Produto inválido", null, -1m)));
    }

    [Fact]
    public async Task CreateAsync_WithMissingName_ThrowsValidationException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(
            () => service.CreateAsync(new CreateProductRequest("  ", null, 10m)));
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ThrowsNotFoundException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateAsync_ChangesProductFields()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateProductRequest("Cadeira", null, 500m));

        var updated = await service.UpdateAsync(
            created.Id,
            new UpdateProductRequest("Cadeira gamer", "Apoio lombar", 750m, IsActive: true));

        Assert.Equal("Cadeira gamer", updated.Name);
        Assert.Equal(750m, updated.Price);
    }

    [Fact]
    public async Task DeleteAsync_RemovesProduct()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateProductRequest("Webcam", null, 199m));

        await service.DeleteAsync(created.Id);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(created.Id));
    }
}
