using System.Net;
using System.Net.Http.Json;
using ApiTemplate.Application.Products;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiTemplate.Tests.Integration;

public sealed class ProductsApiTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetAll_ReturnsSeededProducts()
    {
        var products = await _client.GetFromJsonAsync<List<ProductResponse>>("/api/products");

        Assert.NotNull(products);
        Assert.True(products.Count >= 3);
    }

    [Fact]
    public async Task GetById_WithUnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidPrice_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest("Produto inválido", null, -10m));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreatedProduct()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest("Fone de ouvido", "Bluetooth, cancelamento de ruído", 399.90m));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(created);

        var fetched = await _client.GetFromJsonAsync<ProductResponse>($"/api/products/{created.Id}");

        Assert.NotNull(fetched);
        Assert.Equal("Fone de ouvido", fetched.Name);
        Assert.Equal(399.90m, fetched.Price);
    }

    [Fact]
    public async Task Delete_WithUnknownId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
