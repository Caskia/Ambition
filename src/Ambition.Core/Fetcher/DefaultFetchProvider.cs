using Ambition.Core.Scheduler;
using System;
using System.Collections.Generic;

namespace Ambition.Core.Fetcher
{
    public class DefaultFetchProvider : IFetcherProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultFetchProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Dictionary<Type, Type> ProviderMapper => new Dictionary<Type, Type>()
        {
            { typeof(WebSocketRequestTask), typeof(WebSocketFetcher) },
            { typeof(SocketIORequestTask), typeof(SocketIOFetcher) },
            { typeof(HttpRequestTask), typeof(HttpFetcher) }
        };

        public IFetcher GetTaskFetcher(IRequestTask requestTask)
        {
            var requestTaskType = requestTask.GetType();

            if (!ProviderMapper.ContainsKey(requestTaskType))
            {
                throw new NotSupportedException($"{requestTaskType.Name} not support.");
            }

            return _serviceProvider.GetService(ProviderMapper[requestTaskType]) as IFetcher;
        }
    }
}