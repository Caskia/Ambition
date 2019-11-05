using Ambition.Scheduler;
using System;
using System.Collections.Generic;

namespace Ambition.Fetcher
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

        public void AddOrUpdateFetcher<TRequestTask>(Type fetcherType) where TRequestTask : IRequestTask
        {
            var type = typeof(TRequestTask);

            if (ProviderMapper.ContainsKey(type))
            {
                ProviderMapper[type] = fetcherType;
            }
            else
            {
                ProviderMapper.Add(type, fetcherType);
            }
        }

        public void DeleteFetcher<TRequestTask>() where TRequestTask : IRequestTask
        {
            var type = typeof(TRequestTask);

            if (ProviderMapper.ContainsKey(type))
            {
                ProviderMapper.Remove(type);
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