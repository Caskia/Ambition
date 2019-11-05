using Ambition.Configurations;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Ambition.RabbitMq.MasstransitBus
{
    public class MasstransitBus
    {
        private readonly RabbitMqOptions _options;

        public MasstransitBus(IOptions<RabbitMqOptions> accessorOptions)
        {
            _options = accessorOptions.Value;
            Init();
        }

        public IBusControl BusControl { get; private set; }

        private void Init()
        {
            BusControl = Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                configure.Host(
                    _options.Host,
                    _options.Port,
                    _options.VirtualHost,
                    host =>
                    {
                        host.Username(_options.UserName);
                        host.Password(_options.Password);
                    });
            });
        }
    }
}