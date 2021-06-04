using System;
using System.Collections;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MessageBroker.Kafka.Common;
using MessageBroker.Kafka.Consumer;

namespace MessageBroker.Kafka
{
    public static class ServiceCollectionExtensions
    {
        public static KafkaServiceCollectionWrapper AddKafkaConsumer(this IServiceCollection services,
            params Type[] handlerAssemblyMarkerTypes)
        {
            services.TryAddSingleton<IKafkaTopicCache>(_ => new KafkaTopicCache(services));

            services.AddMediatR(handlerAssemblyMarkerTypes);

            services.AddTransient<IKafkaMessageConsumerManager>(serviceProvider =>
                new KafkaMessageConsumerManager(serviceProvider, services));

            services.AddTransient<IKafkaConsumerBuilder, KafkaConsumerBuilder>();

            services.AddTransient<IKafkaTopicMessageConsumer, KafkaTopicMessageConsumer>();

            services.AddSingleton<IHostedService, KafkaConsumerMessageHandler>();

            return new KafkaServiceCollectionWrapper(services);
        }
    }

    public class KafkaServiceCollectionWrapper : IServiceCollection
    {
        private readonly IServiceCollection serviceCollection;
        private readonly Lazy<IMessageMiddlereManager> messageMiddlewareManagerSingleton;

        public ServiceDescriptor this[int index]
        {
            get => serviceCollection[index];
            set => serviceCollection[index] = value;
        }

        public KafkaServiceCollectionWrapper(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;

            messageMiddlewareManagerSingleton = new Lazy<IMessageMiddlereManager>(() => new MessageMiddlereManager(this.serviceCollection));

            this.serviceCollection.AddSingleton(messageMiddlewareManagerSingleton.Value);
            this.serviceCollection.AddSingleton<IMessageMiddlereExecutorProvider>(prov => prov.GetService(typeof(IMessageMiddlereManager)) as IMessageMiddlereManager);
            this.serviceCollection.AddSingleton<IMessageMiddlewareRegister>(prov => prov.GetService(typeof(IMessageMiddlereManager)) as IMessageMiddlereManager);
        }

        /// <summary>
        /// Try add a scoped middleware to be executed before mediatR publish
        /// </summary>
        /// <typeparam name="TMessage">The IMessage implementation type</typeparam>
        /// <typeparam name="TType">The middleware implementation type</typeparam>
        /// <returns></returns>
        public KafkaServiceCollectionWrapper WithMessageMiddleware<TMessage, TType>()
            where TMessage : IMessage
            where TType: class, IMessageMiddleware<TMessage>
        {
            using (var scope = serviceCollection.BuildServiceProvider())
            {
                scope.GetRequiredService<IMessageMiddlereManager>()
                    .Register<TMessage, TType>();
            }
            return this;
        }

        /// <summary>
        /// Try add a scoped middleware to be executed before mediatR publish
        /// </summary>
        /// <typeparam name="TMessage">The IMessage implementation type</typeparam>
        /// <typeparam name="TType">The middleware implementation type</typeparam>
        /// <param name="factory">The middleware factory</param>
        /// <returns></returns>
        public KafkaServiceCollectionWrapper WithMessageMiddleware<TMessage, TType>(Func<IServiceProvider, TType> factory)
            where TMessage : IMessage
            where TType : class, IMessageMiddleware<TMessage>
        {
            using (var scope = serviceCollection.BuildServiceProvider())
            {
                scope.GetService<IMessageMiddlewareRegister>()
                     .Register<TMessage, TType>(factory);
            }
            return this;
        }

        /// <summary>
        /// Try add a scoped async middleware to be executed before mediatR publish
        /// </summary>
        /// <typeparam name="TMessage">The IMessage implementation type</typeparam>
        /// <typeparam name="TType">The middleware implementation type</typeparam>
        /// <returns></returns>
        public KafkaServiceCollectionWrapper WithAsyncMessageMiddleware<TMessage, TType>()
            where TMessage : IMessage
            where TType : class, IAsyncMessageMiddleware<TMessage>
        {
            using (var scope = serviceCollection.BuildServiceProvider())
            {
                scope.GetService<IMessageMiddlewareRegister>()
                     .RegisterAsync<TMessage, TType>();
            }
            return this;
        }

        /// <summary>
        /// Try add a scoped async middleware to be executed before mediatR publish
        /// </summary>
        /// <typeparam name="TMessage">The IMessage implementation type</typeparam>
        /// <typeparam name="TType">The middleware implementation type</typeparam>
        /// <param name="factory">The middleware factory</param>
        /// <returns></returns>
        public KafkaServiceCollectionWrapper WithAsyncMessageMiddleware<TMessage, TType>(Func<IServiceProvider, TType> factory)
            where TMessage : IMessage
            where TType : class, IAsyncMessageMiddleware<TMessage>
        {
            using (var scope = serviceCollection.BuildServiceProvider())
            {
                scope.GetService<IMessageMiddlewareRegister>()
                     .RegisterAsync<TMessage, TType>(factory);
            }
            return this;
        }

        public int Count => serviceCollection.Count;

        public bool IsReadOnly => serviceCollection.IsReadOnly;

        public void Add(ServiceDescriptor item)
            => serviceCollection.Add(item);

        public void Clear()
            => serviceCollection.Clear();

        public bool Contains(ServiceDescriptor item)
            => serviceCollection.Contains(item);

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
            => serviceCollection.CopyTo(array, arrayIndex);

        public IEnumerator<ServiceDescriptor> GetEnumerator()
            => serviceCollection.GetEnumerator();

        public int IndexOf(ServiceDescriptor item)
            => serviceCollection.IndexOf(item);

        public void Insert(int index, ServiceDescriptor item)
            => serviceCollection.Insert(index, item);

        public bool Remove(ServiceDescriptor item)
            => serviceCollection.Remove(item);

        public void RemoveAt(int index)
            => serviceCollection.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator()
            => serviceCollection.GetEnumerator();
    }
}