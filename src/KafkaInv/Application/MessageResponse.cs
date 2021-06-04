using MessageBroker.Kafka.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaInv.Application
{
    [MessageTopic(TOPIC_NAME)]
    public class MessageResponse : IMessage
    {
        public const string TOPIC_NAME = "topic.name";

        public string Flag { get; set; }
        public long Id { get; set; }
        public DateTime Date { get; set; }

        public MessageResponse()
        {
        }

        public MessageResponse(long id, string flag, DateTime date)
        {
            Flag = flag;
            Id = id;
            Date = date;
        }
    }
}
