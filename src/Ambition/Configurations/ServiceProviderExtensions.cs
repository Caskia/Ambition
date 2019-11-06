using Ambition.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Ambition.Configurations
{
    public static class ServiceProviderExtensions
    {
        public static void UseAmbition(this IServiceProvider serviceProvider)
        {
            serviceProvider.UseAmbitionLog4Net();
        }

        public static void UseAmbitionLog4Net(this IServiceProvider serviceProvider)
        {
            serviceProvider
                 .GetRequiredService<ILoggerFactory>()
                 .AddProvider(new Log4NetLoggerProvider());
        }
    }
}