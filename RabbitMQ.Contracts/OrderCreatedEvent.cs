namespace RabbitMQ.Contracts
{
    public record OrderCreatedEvent(Guid OrderId, string ProductId, int Quantity);

}
