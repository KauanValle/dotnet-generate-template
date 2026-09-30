using ApiTemplate.Application.Products;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTemplate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
