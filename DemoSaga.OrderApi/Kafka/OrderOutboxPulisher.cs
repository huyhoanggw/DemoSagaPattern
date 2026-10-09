using DemoSaga.OrderApi.Database;
using DemoSaga.OrderService.Entities;
using DemoSaga.Shared;
using Microsoft.EntityFrameworkCore;

namespace DemoSaga.OrderService.Kafka
{
    public class OrderOutboxPulisher<TContext>(IServiceProvider serviceProvider, ILogger<OrderOutboxPulisher<TContext>> logger) : BackgroundService where TContext : DbContext
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await pulishAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while publishing outbox messages.");
                }
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

            }
        }

        private async Task pulishAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceProvider.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<TContext>();
            var producer = scope.ServiceProvider.GetRequiredService<IKafkaProducer>();
            // lay ra 10 ban ghi chua duoc publish
            var messages = await dbcontext.Set<OrderOutBoxMessage>().Where(x => x.PublishedAtUtc == null)
                .OrderBy(x => x.CreatedAtUtc).ThenBy(x => x.Id).Take(10).ToListAsync(stoppingToken);
            foreach (var message in messages)
            {
                try
                {
                    await producer.ProduceRawAsync<OrderDbContext>(message.Topic, message.Id, message.Payload);
                    message.PublishedAtUtc = DateTime.UtcNow;
                    await dbcontext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while publishing outbox message with Id {MessageId}.", message.Id);
                }
            }
        }
    }
}
