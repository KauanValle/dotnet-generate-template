using ApiTemplate.Api.ExceptionHandling;
using ApiTemplate.Application;
using ApiTemplate.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Framework services
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.Services.EnsureDatabaseCreatedAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Swagger-style UI at /scalar
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

/// <summary>
/// Exposed for WebApplicationFactory in integration tests.
/// </summary>
public partial class Program { }
