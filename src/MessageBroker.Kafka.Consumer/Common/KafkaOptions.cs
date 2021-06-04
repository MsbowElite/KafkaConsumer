using Confluent.Kafka;

namespace MessageBroker.Kafka.Common
{
    public class KafkaOptions
    {
        public string KafkaBootstrapServers { get; set; }
        public string ConsumerGroupId { get; set; }
        public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;
    }
}