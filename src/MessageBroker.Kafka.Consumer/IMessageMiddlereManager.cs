using System;
using MessageBroker.Kafka.Common;

namespace MessageBroker.Kafka.Consumer
{
    public interface IMessageMiddlereExecutorProvider
    {
        Action<object> CreateExecutor(IServiceProvider serviceProvider, Type messageType);
    }

    public interface IMessageMiddlewareRegister
    {
        void Register<TMessage, TType>()
            where TMessage : IMessage
            where TType : class, IMessageMiddleware<TMessage>;

        void Register<TMessage, TType>(Func<IServiceProvider, TType> factory)
            where TMessage : IMessage
            where TType : class, IMessageMiddleware<TMessage>;

        void RegisterAsync<TMessage, TType>()
            where TMessage : IMessage
            where TType : class, IAsyncMessageMiddleware<TMessage>;

        void RegisterAsync<TMessage, TType>(Func<IServiceProvider, TType> factory)
            where TMessage : IMessage
            where TType : class, IAsyncMessageMiddleware<TMessage>;
    }

    public interface IMessageMiddlereManager : IMessageMiddlereExecutorProvider, IMessageMiddlewareRegister
    {

    }
}
