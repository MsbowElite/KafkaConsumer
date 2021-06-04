
using KafkaInv.Application;
using MediatR;
using MessageBroker.Kafka.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaInv.Handlers.Messages
{
    public class MessageResponseHandler : INotificationHandler<MessageNotification<MessageResponse>>
    {
        public MessageResponseHandler()
        {

        }

        public async Task Handle(MessageNotification<MessageResponse> notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("Message arrived");
            Console.WriteLine(notification);
        }
    }
}

