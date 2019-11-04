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

        public Dictionary<Type, Type> ProviderMapper { get; } = new Dictionary<Type, Type>()
        {
            { typeof(WebSocketRequestTask), typeof(WebSocketFetcher) },
            { typeof(SocketIORequestTask), typeof(SocketIOFetcher) },
            { typeof(HttpRequestTask), typeof(HttpFetcher) }
        };

        public void AddOrUpdateFetcher(Type requestTaskType, Type fetcherType)
        {
            if (ProviderMapper.ContainsKey(requestTaskType))
            {
                ProviderMapper[requestTaskType] = fetcherType;
            }
            else
            {
                ProviderMapper.Add(requestTaskType, fetcherType);
            }
        }

        public void DeleteFetcher(Type requestTaskType)
        {
            if (ProviderMapper.ContainsKey(requestTaskType))
            {
                ProviderMapper.Remove(requestTaskType);
            }
        }

        public IFetcher GetFetcher(Type requestTaskType)
        {
            if (!ProviderMapper.ContainsKey(requestTaskType))
            {
                throw new NotSupportedException($"{requestTaskType.Name} not support.");
            }

            return _serviceProvider.GetService(ProviderMapper[requestTaskType]) as IFetcher;
        }
    }
}