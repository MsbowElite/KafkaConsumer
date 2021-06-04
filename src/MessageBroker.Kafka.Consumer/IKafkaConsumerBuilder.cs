using Confluent.Kafka;

namespace MessageBroker.Kafka.Consumer
{
    public interface IKafkaConsumerBuilder
    {
        IConsumer<string, string> Build();
    }
}