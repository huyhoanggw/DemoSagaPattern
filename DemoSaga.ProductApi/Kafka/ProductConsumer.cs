using Confluent.Kafka;
using DemoSaga.Models;
using DemoSaga.ProductService.Database;
using DemoSaga.Shared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DemoSaga.ProductService.Kafka
{
    public class ProductConsumer : KafkaConsumer
    {
        private static readonly string[] Topics = ["order-created"];
        private IServiceScopeFactory _scopeFactory;
        private IKafkaProducer _kafkaProducer;
        private ILogger<ProductConsumer> _logger;
        private static string groupId = "product-group";

        public ProductConsumer(IServiceScopeFactory scopeFactory, IKafkaProducer kakfaProducer, ILogger<ProductConsumer> logger) : base(Topics, logger, groupId)
        {
            _scopeFactory = scopeFactory;
            _kafkaProducer = kakfaProducer;
            _logger = logger;
        }
        protected override async Task ConsumeAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken)
        {
            try
            {
                switch (consumeResult.Topic)
                {
                    case "order-created":
                        await HandlerOrderCreated(consumeResult.Message.Value, cancellationToken);
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                // Bỏ qua lỗi này khi ứng dụng bị ngắt/shutdown bình thường
                _logger.LogWarning("Consume task was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Kafka message from topic: {Topic}", consumeResult.Topic);
            }

        }

        private async Task HandlerOrderCreated(string message, CancellationToken cancellationToken)
        {
            var order = JsonConvert.DeserializeObject<OrderModel>(message);
            await ReservedProduct(order, CancellationToken.None);
        }

        public async Task<bool> ReservedProduct(OrderModel? order, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var _dbcontext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
            var product = await _dbcontext.Products.Where(x => x.Id == order.ProductId).FirstOrDefaultAsync();
            if (product is not null && product.Quantity >= order.Quantity)
            {
                product.Quantity -= order.Quantity;
                _dbcontext.ProductOutBoxMessages.Add(new Entities.ProductOutBoxMessage()
                {
                    Id = Guid.NewGuid(),
                    Topic = "product-reserved",
                    Payload = JsonConvert.SerializeObject(order),
                    CreatedAtUtc = DateTime.UtcNow
                });


                await _dbcontext.SaveChangesAsync();
                return true;

            }
            else
            {
                _dbcontext.ProductOutBoxMessages.Add(new Entities.ProductOutBoxMessage()
                {
                    Id = Guid.NewGuid(),
                    Topic = "product-reserved-failed",
                    Payload = JsonConvert.SerializeObject(order),
                    CreatedAtUtc = DateTime.UtcNow
                });
                return false;
            }
        }
    }
}
