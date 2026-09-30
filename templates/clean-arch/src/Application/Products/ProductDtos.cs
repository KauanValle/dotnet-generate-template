namespace ApiTemplate.Application.Products;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price);

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    bool IsActive);
