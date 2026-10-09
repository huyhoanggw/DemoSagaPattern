using DemoSaga.ProductService.Database;
using DemoSaga.ProductService.Entities;
using DemoSaga.Shared;
using Microsoft.EntityFrameworkCore;

namespace DemoSaga.ProductService.Kafka
{
    public class ProductOutboxPulisher<Tcontext>(IServiceProvider serviceProvider, ILogger<ProductOutboxPulisher<Tcontext>> logger) : BackgroundService where Tcontext : DbContext
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
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

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
            var dbcontext = scope.ServiceProvider.GetRequiredService<Tcontext>();
            var producer = scope.ServiceProvider.GetRequiredService<IKafkaProducer>();
            var messages = await dbcontext.Set<ProductOutBoxMessage>().Where(x => x.PublishedAtUtc == null)
                .OrderBy(x => x.CreatedAtUtc).ThenBy(x => x.Id).Take(10).ToListAsync(stoppingToken);
            foreach (var message in messages)
            {
                try
                {
                    await producer.ProduceRawAsync<ProductDbContext>(message.Topic, message.Id, message.Payload);
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
