using Ambition.Pipeline;
using Ambition.RabbitMq.MasstransitBus;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ambition.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAmbitionRabbitMq(this IServiceCollection services, Action<RabbitMqOptions> optionsAction)
        {
            services.Configure(optionsAction);
            services.AddSingleton<MasstransitBus>();
            services.AddSingleton<RabbitMqPipeline>();

            return services;
        }
    }
}