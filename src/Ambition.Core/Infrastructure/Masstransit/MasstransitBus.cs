using MassTransit;
using System;

namespace Ambition.Core.Infrastructure
{
    public sealed class MasstransitBus
    {
        public IBusControl BusControl;
        private readonly static Lazy<MasstransitBus> _lazy = new Lazy<MasstransitBus>(() => new MasstransitBus());

        private MasstransitBus()
        {
            BusControl = Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                configure.Host(
                    ConfigurationManager.Instance.Root["RabbitMq:Host"],
                    Convert.ToUInt16(ConfigurationManager.Instance.Root["RabbitMq:Port"]),
                    ConfigurationManager.Instance.Root["RabbitMq:VirtualHost"],
                    host =>
                    {
                        host.Username(ConfigurationManager.Instance.Root["RabbitMq:UserName"]);
                        host.Password(ConfigurationManager.Instance.Root["RabbitMq:Password"]);
                    });
            });
        }

        public static MasstransitBus Instance
        {
            get
            {
                return _lazy.Value;
            }
        }
    }
}