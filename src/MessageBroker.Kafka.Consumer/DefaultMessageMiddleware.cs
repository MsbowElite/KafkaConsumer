using System;
using System.Threading.Tasks;
using MessageBroker.Kafka.Common;

namespace MessageBroker.Kafka.Consumer
{
    public class DefaultMessageMiddleware<TMessage> : IDefaultMessageMiddleware<TMessage>
        where TMessage: IMessage
    {
        private readonly Action<IServiceProvider, TMessage> _action;

        public DefaultMessageMiddleware(Action<IServiceProvider, TMessage> action)
        {
            _action = action ?? throw new ArgumentNullException("action");
        }

        public void Invoke(IServiceProvider provider, TMessage message)
        {
            _action(provider, message);
        }
    }

    public class DefaultAsyncMessageMiddleware<TMessage> : IDefaultAsyncMessageMiddleware<TMessage>
        where TMessage : IMessage
    {
        private readonly Func<IServiceProvider, TMessage, Task> _func;

        public DefaultAsyncMessageMiddleware(Func<IServiceProvider, TMessage, Task> func)
        {
            _func = func ?? throw new ArgumentNullException("func");
        }

        public async Task InvokeAsync(IServiceProvider provider, TMessage message)
        {
            await _func(provider, message);
        }
    }
}
