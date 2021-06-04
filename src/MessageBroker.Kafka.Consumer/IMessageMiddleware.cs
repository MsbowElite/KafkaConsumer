using System;
using System.Threading.Tasks;
using MessageBroker.Kafka.Common;

namespace MessageBroker.Kafka.Consumer
{
    public interface IMessageMiddleware<TType>
        where TType: IMessage
    {
        void Invoke(TType message);
    }

    public interface IAsyncMessageMiddleware<TType>
        where TType : IMessage
    {
        Task InvokeAsync(TType message);
    }

    public interface IDefaultMessageMiddleware<TType>
        where TType : IMessage
    {
        void Invoke(IServiceProvider provider, TType message);
    }

    public interface IDefaultAsyncMessageMiddleware<TType>
        where TType : IMessage
    {
        Task InvokeAsync(IServiceProvider provider, TType message);
    }
}
