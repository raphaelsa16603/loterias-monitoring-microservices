using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Loterias.Messaging.Interfaces;
using Loterias.Messaging.Kafka;

namespace Loterias.Messaging.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaMessaging(this IServiceCollection services, Action<KafkaSettings> configure)
        {
            var settings = new KafkaSettings();
            configure(settings);

            services.AddSingleton(settings);
            services.AddSingleton<IMessageProducer, KafkaProducer>();
            services.AddSingleton<IMessageConsumer, KafkaConsumer>();

            return services;
        }
    }
}

