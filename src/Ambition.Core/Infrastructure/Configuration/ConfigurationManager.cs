using Ambition.Core.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Ambition.Core.Infrastructure
{
    public sealed class ConfigurationManager
    {
        private readonly static Lazy<ConfigurationManager> _lazy = new Lazy<ConfigurationManager>(() => new ConfigurationManager());

        private ConfigurationManager()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!environmentName.IsNullOrWhiteSpace())
            {
                builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
            }

            Root = builder.Build();
        }

        public static ConfigurationManager Instance
        {
            get
            {
                return _lazy.Value;
            }
        }

        public IConfigurationRoot Root { get; set; }
    }
}