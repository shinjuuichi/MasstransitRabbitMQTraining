using MassTransit;
using ProductService.Data;
using RabbitMQ.Contracts;
namespace ProductService.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ProductDbContext _context;

        public OrderCreatedConsumer(ProductDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            Console.WriteLine($"[ProductService] Received OrderCreatedEvent for Product {context.Message.ProductId}, Quantity {context.Message.Quantity}");

            var product = await _context.Products.FindAsync(context.Message.ProductId);
            if (product != null)
            {
                Console.WriteLine($"[ProductService] Updating stock for Product {product.Id}. Current Stock: {product.Stock}");

                product.Stock -= context.Message.Quantity;
                await _context.SaveChangesAsync();

                Console.WriteLine($"[ProductService] Stock Updated for Product {product.Id}. New Stock: {product.Stock}");
            }
            else
            {
                Console.WriteLine($"[ProductService] Product {context.Message.ProductId} NOT FOUND in database!");
            }
        }

    }
}
