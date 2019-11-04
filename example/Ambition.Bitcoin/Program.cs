using Ambition.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Ambition.Bitcoin
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddAmbition();
                services.AddHostedService<HostedService>();
            })
            .Build();

            await host.RunAsync();
        }
    }
}