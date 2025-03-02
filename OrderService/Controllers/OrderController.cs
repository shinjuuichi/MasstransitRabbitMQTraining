using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Data;
using RabbitMQ.Contracts;

[Route("api/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderController(OrderDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var orderEvent = new OrderCreatedEvent(order.Id, order.ProductId, order.Quantity);

        Console.WriteLine($"[OrderService] Publishing OrderCreatedEvent for Product {order.ProductId}, Quantity {order.Quantity}");

        await _publishEndpoint.Publish(orderEvent);

        Console.WriteLine("[OrderService] OrderCreatedEvent published successfully.");

        return Ok(order);
    }
}
