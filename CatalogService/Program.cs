using Microsoft.AspNetCore.Mvc;
using SharedContracts;
using Microsoft.AspNetCore.OpenApi;


var builder = WebApplication.CreateBuilder(args);

// Add application abstractions and services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CatalogRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// REST API Minimal Routing Definitions
app.MapGet("/api/catalog", (CatalogRepository repo) => Results.Ok(repo.GetAllItems()))
   .WithName("GetCatalogItems")
   .WithOpenApi();

app.MapGet("/api/catalog/{id:guid}", (Guid id, CatalogRepository repo) => 
{
    var item = repo.GetById(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
})
.WithName("GetCatalogItemById")
.WithOpenApi();

app.MapPost("/api/catalog", ([FromBody] CreateProductDto dto, CatalogRepository repo) => 
{
    var newProduct = repo.Create(dto.Name, dto.SKU, dto.Price);
    
    // In a real microservice system, you would broadcast the integration event here using an Event Bus (e.g., RabbitMQ/MassTransit)
    app.Logger.LogInformation("Integration Event Dispatched: ProductPriceChangedIntegrationEvent -> New SKU: {SKU}, Price: {Price}", newProduct.SKU, newProduct.Price);
    
    return Results.Created($"/api/catalog/{newProduct.Id}", newProduct);
})
.WithName("CreateCatalogItem")
.WithOpenApi();

app.Run();

// In-Memory Domain Layer Models & Datastores
public record Product(Guid Id, string Name, string SKU, decimal Price, DateTime CreatedAt);
public record CreateProductDto(string Name, string SKU, decimal Price);

public class CatalogRepository
{
    private readonly List<Product> _products = new()
    {
        new Product(Guid.NewGuid(), "Cloud Native Compute Framework Book", "BK-CNCF-01", 45.99m, DateTime.UtcNow),
        new Product(Guid.NewGuid(), "Distributed Architecture Poster", "PST-DIST-09", 19.50m, DateTime.UtcNow),
        new Product(Guid.NewGuid(), "Containerization Sticker Pack", "STK-CONT-05", 5.00m, DateTime.UtcNow)
    };

    public IEnumerable<Product> GetAllItems() => _products;
    
    public Product? GetById(Guid id) => _products.FirstOrDefault(p => p.Id == id);
    
    public Product Create(string name, string sku, decimal price)
    {
        var item = new Product(Guid.NewGuid(), name, sku, price, DateTime.UtcNow);
        _products.Add(item);
        return item;
    }
}
