using KafkaInv.Application;
using KafkaInv.Handlers.Messages;
using MessageBroker.Kafka;
using MessageBroker.Kafka.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaInv.Configurations
{
    public static class KafkaConsumer
    {
        public static IServiceCollection RegisterKafkaConsumer(this IServiceCollection services)
        {
            services.AddOptions<KafkaOptions>()
                .Configure(opt =>
                {
                    opt.KafkaBootstrapServers = "localhost:9092";
                    opt.ConsumerGroupId = "group.id";
                });

            return services.AddKafkaConsumer(typeof(MessageResponse))
                                       .WithMessageMiddleware<MessageResponse, ResponseMiddleware>();
        }
    }
}
