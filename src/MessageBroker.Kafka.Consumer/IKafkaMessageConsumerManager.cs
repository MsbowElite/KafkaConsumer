using System.Threading;

namespace MessageBroker.Kafka.Consumer
{
    public interface IKafkaMessageConsumerManager
    {
        void StartConsumers(CancellationToken cancellationToken);
    }
}