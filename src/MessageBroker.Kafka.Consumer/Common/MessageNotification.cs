using MediatR;
using System;

namespace MessageBroker.Kafka.Common
{
    public class MessageNotification<TMessage> : INotification
        where TMessage : IMessage
    {
        public MessageNotification(TMessage message)
        {
            Message = message ?? throw new ArgumentNullException("message", "Message must not be null.");
        }

        public TMessage Message { get; }

        public override bool Equals(object obj)
        {
            if (obj is MessageNotification<TMessage> messageEnvelop)
            {
                return Message.Equals(messageEnvelop.Message);
            }
            if (obj is TMessage message)
            {
                return Message.Equals(message);
            }
            return false;
        }

        public override int GetHashCode()
            => Message.GetHashCode();

        public static explicit operator TMessage(MessageNotification<TMessage> message)
        {
            if (message is not null)
                return message.Message;
            return default(TMessage);
        }

        public static explicit operator MessageNotification<TMessage>(TMessage message)
            => new MessageNotification<TMessage>(message);

        public static bool operator ==(MessageNotification<TMessage> left, MessageNotification<TMessage> right)
        {
            if (left is null && right is null)
                return true;
            if (left is null || right is null)
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(MessageNotification<TMessage> left, MessageNotification<TMessage> right)
            => !(left == right);

        public static bool operator ==(MessageNotification<TMessage> left, TMessage right)
        {
            if (left is null && right is null)
                return true;
            if (left is null || right is null)
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(MessageNotification<TMessage> left, TMessage right)
            => !(left == right);

        public static bool operator ==(TMessage left, MessageNotification<TMessage> right)
        {
            if (left is null && right is null)
                return true;
            if (left is null || right is null)
                return false;
            return right.Equals(left);
        }

        public static bool operator !=(TMessage left, MessageNotification<TMessage> right)
            => !(left == right);
    }
}