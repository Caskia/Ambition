﻿using Ambition.Bitcoin.Processors;
using Ambition.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Ambition.Bitcoin.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSpider(this IServiceCollection services)
        {
            services.AddAmbition();
            services.AddSingleton<BtcToolsFetchResultProcessor>();
            services.AddSingleton<FilePipeline>();

            return services;
        }
    }
}