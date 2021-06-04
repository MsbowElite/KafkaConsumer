using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using MessageBroker.Kafka.Common;

namespace MessageBroker.Kafka.Consumer
{
    public class MessageMiddlereManager : IMessageMiddlereManager, IDisposable
    {
        private readonly IServiceCollection _serviceCollection;

        private readonly ConcurrentDictionary<Type, ConcurrentBag<Type>> messageMiddlewareTypes
            = new ConcurrentDictionary<Type, ConcurrentBag<Type>>();

        private readonly ConcurrentDictionary<Type, ConcurrentBag<Type>> messageAsyncMiddlewareTypes
            = new ConcurrentDictionary<Type, ConcurrentBag<Type>>();

        public MessageMiddlereManager(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public Action<object> CreateExecutor(IServiceProvider serviceProvider, Type messageType)
        {
            if (!messageMiddlewareTypes.ContainsKey(messageType) &&
                !messageAsyncMiddlewareTypes.ContainsKey(messageType))
                return mesage => { };

            return message =>
            {
                if (messageMiddlewareTypes.TryGetValue(messageType, out var middlewareTypes))
                {
                    foreach(var middlewareType in middlewareTypes)
                    {
                        var middleware = serviceProvider.GetService(middlewareType);
                        try
                        {
                            middlewareType
                                .GetMethod("Invoke")
                                .Invoke(middleware, new object[] { message });
                        }
                        catch
                        {

                        }
                    }
                }

                if (messageAsyncMiddlewareTypes.TryGetValue(messageType, out var asyncMiddlewareTypes))
                {
                    foreach (var asyncMiddlewareType in asyncMiddlewareTypes)
                    {
                        var middleware = serviceProvider.GetService(asyncMiddlewareType);
                        try
                        {
                            Task.WaitAll(asyncMiddlewareType
                                .GetMethod("InvokeAsync")
                                .Invoke(middleware, new object[] { message }) as Task);
                        }
                        catch
                        {

                        }
                    }
                }
            };
        }

        public void Dispose()
        {
            Console.WriteLine("Finalizando");
        }

        public void Register<TMessage, TType>()
            where TMessage : IMessage
            where TType : class, IMessageMiddleware<TMessage>
        {
            var middlewareType = typeof(TType);
            var messageType = typeof(TMessage);

            if (messageMiddlewareTypes.TryGetValue(messageType, out var middlewares) &&
                !middlewares.ToList().Contains(middlewareType))
            {
                middlewares.Add(middlewareType);
            }
            else
            {
                if (!messageMiddlewareTypes.TryAdd(messageType, new ConcurrentBag<Type> { middlewareType }))
                    throw new InvalidOperationException($"Não foi possível registrar o middleware do tipo '{middlewareType.FullName}'");
            }
            _serviceCollection.TryAddScoped<TType>();
        }

        public void Register<TMessage, TType>(Func<IServiceProvider, TType> factory)
            where TMessage : IMessage
            where TType : class, IMessageMiddleware<TMessage>
        {
            var middlewareType = typeof(TType);
            var messageType = typeof(TMessage);

            if (messageMiddlewareTypes.TryGetValue(messageType, out var middlewares) &&
                !middlewares.ToList().Contains(middlewareType))
            {
                middlewares.Add(middlewareType);
            }
            else
            {
                if (!messageMiddlewareTypes.TryAdd(messageType, new ConcurrentBag<Type> { middlewareType }))
                    throw new InvalidOperationException($"Não foi possível registrar o middleware do tipo '{middlewareType.FullName}'");
            }

            if (factory is not default(Func<IServiceProvider, TType>))
                _serviceCollection.TryAddScoped(factory);
            else
                _serviceCollection.TryAddScoped<TType>();
        }

        public void RegisterAsync<TMessage, TType>()
            where TMessage : IMessage
            where TType : class, IAsyncMessageMiddleware<TMessage>
        {
            var middlewareType = typeof(TType);
            var messageType = typeof(TMessage);

            if (messageAsyncMiddlewareTypes.TryGetValue(messageType, out var middlewares) &&
                !middlewares.ToList().Contains(middlewareType))
            {
                middlewares.Add(middlewareType);
            }
            else
            {
                if (!messageAsyncMiddlewareTypes.TryAdd(messageType, new ConcurrentBag<Type> { middlewareType }))
                    throw new InvalidOperationException($"Não foi possível registrar o middleware assíncrono do tipo '{middlewareType.FullName}'");
            }
            _serviceCollection.TryAddScoped<TType>();
        }

        public void RegisterAsync<TMessage, TType>(Func<IServiceProvider, TType> factory)
            where TMessage : IMessage
            where TType : class, IAsyncMessageMiddleware<TMessage>
        {
            var middlewareType = typeof(TType);
            var messageType = typeof(TMessage);

            if (messageMiddlewareTypes.TryGetValue(messageType, out var middlewares) &&
                !middlewares.ToList().Contains(middlewareType))
            {
                middlewares.Add(middlewareType);
            }
            else
            {
                if (!messageMiddlewareTypes.TryAdd(messageType, new ConcurrentBag<Type> { middlewareType }))
                    throw new InvalidOperationException($"Não foi possível registrar o middleware assíncrono do tipo '{middlewareType.FullName}'");
            }

            if (factory is not default(Func<IServiceProvider, TType>))
                _serviceCollection.TryAddScoped(factory);
            else
                _serviceCollection.TryAddScoped<TType>();
        }
    }
}
