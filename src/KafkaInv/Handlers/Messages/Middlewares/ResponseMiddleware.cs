using KafkaInv.Application;
using MessageBroker.Kafka.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaInv.Handlers.Messages
{
    public class ResponseMiddleware : IMessageMiddleware<MessageResponse>
    {
        public ResponseMiddleware() { }

        public void Invoke(MessageResponse message)
        {

        }
    }
}
