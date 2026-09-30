using ApiTemplate.Application.Common.Exceptions;
using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Application.Products;

public sealed class ProductService(IRepository<Product> repository) : IProductService
{
    public async Task<List<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllAsync(cancellationToken);
        return products.Select(ToResponse).ToList();
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await FindProductOrThrowAsync(id, cancellationToken);
        return ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Price);

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price
        };

        repository.Add(product);
        await repository.SaveChangesAsync(cancellationToken);

        return ToResponse(product);
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Price);

        var product = await FindProductOrThrowAsync(id, cancellationToken);
        product.Name = request.Name.Trim();
        product.Description = request.Description?.Trim();
        product.Price = request.Price;
        product.IsActive = request.IsActive;

        repository.Update(product);
        await repository.SaveChangesAsync(cancellationToken);

        return ToResponse(product);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await FindProductOrThrowAsync(id, cancellationToken);
        repository.Remove(product);
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Product> FindProductOrThrowAsync(Guid id, CancellationToken cancellationToken)
        => await repository.FindByIdAsync(id, cancellationToken)
           ?? throw new NotFoundException(nameof(Product), id);

    private static void Validate(string name, decimal price)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors[nameof(CreateProductRequest.Name)] = ["Name is required."];
        }

        if (price <= 0)
        {
            errors[nameof(CreateProductRequest.Price)] = ["Price must be greater than zero."];
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }

    private static ProductResponse ToResponse(Product product)
        => new(product.Id, product.Name, product.Description, product.Price, product.IsActive, product.CreatedAtUtc);
}
