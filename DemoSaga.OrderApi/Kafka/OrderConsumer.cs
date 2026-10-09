using Confluent.Kafka;
using DemoSaga.OrderApi.Database;
using DemoSaga.OrderService.Entities;
using DemoSaga.Shared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace DemoSaga.OrderService.Kafka
{
    public class OrderConsumer : KafkaConsumer
    {
        public static readonly string[] Topics = ["product-reserved", "product-reserved-failed"];

        public ILogger<OrderConsumer> _logger;
        private IServiceProvider _serviceProvider;
        private static string groupId = "order-group";
        public OrderConsumer(ILogger<OrderConsumer> logger, IServiceProvider serviceProvider) : base(Topics, logger, groupId)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ConsumeAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken)
        {
            base.ConsumeAsync(consumeResult, cancellationToken);
            try
            {
                switch (consumeResult.Topic)
                {
                    case "product-reserved":
                        await HandlerProductReserved(consumeResult.Message.Value, cancellationToken);
                        _logger.LogInformation("Product reserved successfully for order: {OrderId}", consumeResult.Message.Value);
                        break;
                    case "product-reserved-failed":
                        await HandlerCancelOrder(consumeResult.Message.Value, cancellationToken);
                        _logger.LogWarning("Product reservation failed for order: {OrderId}", consumeResult.Message.Value);
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

        private async Task HandlerCancelOrder(string message, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            var order = JsonConvert.DeserializeObject<Order>(message);
            var exisitOrder = await dbcontext.Orders.Where(x => x.Id == order.Id).FirstOrDefaultAsync(cancellationToken);
            if (exisitOrder != null)
            {
                exisitOrder.Status = "Cancelled";
                dbcontext.Orders.Update(exisitOrder);
                await dbcontext.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task HandlerProductReserved(string message, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            var order = JsonConvert.DeserializeObject<Order>(message);
            var exisitOrder = await dbcontext.Orders.Where(x => x.Id == order.Id).FirstOrDefaultAsync(cancellationToken);
            if (exisitOrder != null)
            {
                exisitOrder.Status = "Confirmed";
                dbcontext.Orders.Update(exisitOrder);
                await dbcontext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
