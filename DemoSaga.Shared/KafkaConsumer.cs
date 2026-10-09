using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoSaga.Shared
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly ILogger _logger;
        public KafkaConsumer(string[] topics, ILogger logger, string groupId)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _consumer.Subscribe(topics);
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                await HandlerConsume(stoppingToken);
            }, stoppingToken);
        }

        private async Task HandlerConsume(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    await ConsumeAsync(consumeResult, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogWarning("Kafka Consume Warning: {Reason}. Thử lại sau 5 giây...", ex.Error.Reason);
                    await Task.Delay(5000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Bỏ qua khi ứng dụng shutdown bình thường
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi không xác định trong KafkaConsumer");
                    await Task.Delay(2000, stoppingToken);
                }
            }
        }

        protected virtual Task ConsumeAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
