namespace DemoSaga.OrderService.Entities
{
    public class OrderOutBoxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Topic { get; set; } = "";
        public string Payload { get; set; } = "";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAtUtc { get; set; }
    }
}
