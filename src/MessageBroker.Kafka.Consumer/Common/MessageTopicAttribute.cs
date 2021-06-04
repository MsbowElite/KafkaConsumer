using System;

namespace MessageBroker.Kafka.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MessageTopicAttribute : Attribute
    {
        public MessageTopicAttribute(string topic)
        {
            Topic = topic;
        }

        public string Topic { get; }
    }
}