using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using MessageBroker.Kafka.Common;

namespace MessageBroker.Kafka.Consumer
{
    public class KafkaTopicMessageConsumer : IKafkaTopicMessageConsumer
    {
        private readonly IKafkaConsumerBuilder _kafkaConsumerBuilder;
        private readonly IServiceProvider _serviceProvider;
        private readonly IKafkaTopicCache _cache;
        private readonly IMessageMiddlereExecutorProvider _middlewareManager;

        public KafkaTopicMessageConsumer(IKafkaConsumerBuilder kafkaConsumerBuilder, IServiceProvider serviceProvider, IKafkaTopicCache cache, IMessageMiddlereExecutorProvider middlewareManager)
        {
            _kafkaConsumerBuilder = kafkaConsumerBuilder;
            _serviceProvider = serviceProvider;
            _cache = cache;
            _middlewareManager = middlewareManager;
        }

        public void StartConsuming(string topic, CancellationToken cancellationToken)
        {
            using (var consumer = _kafkaConsumerBuilder.Build())
            {
                consumer.Subscribe(topic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cancellationToken);
                            var messageTypes = _cache[topic];
                            if (messageTypes.Count > 0)
                            {
                                foreach (var messageType in messageTypes)
                                {
                                    try
                                    {
                                        var message = JsonConvert.DeserializeObject(consumeResult.Message.Value, messageType);
                                        var messageNotificationType = typeof(MessageNotification<>).MakeGenericType(messageType);
                                        var messageNotification = Activator.CreateInstance(messageNotificationType, message);
                                        using (var scope = _serviceProvider.CreateScope())
                                        {
                                            ExecuteMiddlewaresIfExists(scope.ServiceProvider, messageType, message);
                                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                                            mediator.Publish(messageNotification, cancellationToken).GetAwaiter().GetResult();
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // do nothing on cancellation
                }
                finally
                {
                    consumer.Close();
                }
            }
        }

        private void ExecuteMiddlewaresIfExists(IServiceProvider services, Type messageType, object message)
        {
            try
            {
                var middlewareExecutor = _middlewareManager.CreateExecutor(services, messageType);
                middlewareExecutor(message);
            }
            catch
            {
            }
        }
    }
}