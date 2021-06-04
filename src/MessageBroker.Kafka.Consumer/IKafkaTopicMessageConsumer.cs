using System.Threading;

namespace MessageBroker.Kafka.Consumer
{
    public interface IKafkaTopicMessageConsumer
    {
        void StartConsuming(string topic, CancellationToken cancellationToken);
    }
}