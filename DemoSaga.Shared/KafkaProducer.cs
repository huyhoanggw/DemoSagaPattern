using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DemoSaga.Shared
{
    public interface IKafkaProducer
    {
        Task ProduceAsync<T>(string topic, T message);
        Task ProduceRawAsync<T>(string topic, Guid EventId, string Payload);
    }
    public class KafkaProducer : IKafkaProducer
    {
        public IProducer<string, string> _producer;
        public KafkaProducer()
        {
            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092",

            };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        public Task ProduceAsync<T>(string topic, T message)
        {
            var kafkaMessage = new Message<string, string> { Value = JsonConvert.SerializeObject(message) };
            return _producer.ProduceAsync(topic, kafkaMessage);
        }

        public Task ProduceRawAsync<T>(string topic, Guid EventId, string Payload)
        {
            var kafkaMessage = new Message<string, string> { Key = EventId.ToString(), Value = Payload };
            return _producer.ProduceAsync(topic, kafkaMessage);
        }
    }
}
