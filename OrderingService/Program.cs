using Microsoft.AspNetCore.Mvc;
using SharedContracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<OrderStore>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Routing definitions demonstrating inter-service execution context
app.MapGet("/api/orders", (OrderStore store) => Results.Ok(store.GetAllOrders()))
   .WithName("GetOrders")
   .WithOpenApi();

app.MapPost("/api/orders", ([FromBody] SubmitOrderDto dto, OrderStore store) => 
{
    var newOrder = store.PlaceOrder(dto.ProductId, dto.Quantity, dto.UnitPrice);
    
    // Logging internal business events demonstrating asynchronous communication modeling
    app.Logger.LogInformation("Order successfully processed internally. Order ID: {OrderId}. total value: {Total}", newOrder.OrderId, newOrder.TotalAmount);
    
    return Results.Created($"/api/orders/{newOrder.OrderId}", newOrder);
})
.WithName("PlaceOrder")
.WithOpenApi();

app.Run();

// Models and In-Memory tracking store
public record OrderRecord(Guid OrderId, Guid ProductId, int Quantity, decimal TotalAmount, string OrderStatus, DateTime ProcessedAt);
public record SubmitOrderDto(Guid ProductId, int Quantity, decimal UnitPrice);

public class OrderStore
{
    private readonly List<OrderRecord> _orders = new();

    public IEnumerable<OrderRecord> GetAllOrders() => _orders;

    public OrderRecord PlaceOrder(Guid productId, int quantity, decimal unitPrice)
    {
        var record = new OrderRecord(
            OrderId: Guid.NewGuid(),
            ProductId: productId,
            Quantity: quantity,
            TotalAmount: quantity * unitPrice,
            OrderStatus: "Accepted/PendingProcessing",
            ProcessedAt: DateTime.UtcNow
        );
        _orders.Add(record);
        return record;
    }
}
