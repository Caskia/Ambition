using Ambition.Bitcoin.Configurations;
using Ambition.Configurations;
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
                services.AddSpider();
                services.AddHostedService<HostedService>();
            })
            //.ConfigureAppConfiguration((hostingContext, config) =>
            //{
            //    var env = hostingContext.HostingEnvironment;

            //    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //        .AddEnvironmentVariables();
            //})
            .Build();

            host.Services.UseAmbition();

            await host.RunAsync();
        }
    }
}