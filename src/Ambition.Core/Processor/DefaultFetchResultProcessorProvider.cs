using Ambition.Core.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ambition.Core.Processor
{
    public class DefaultFetchResultProcessorProvider : IFetchResultProcessorProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultFetchResultProcessorProvider(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        public Dictionary<Type, List<Type>> ProviderMapper { get; } = new Dictionary<Type, List<Type>>()
        {
            { typeof(WebSocketRequestTask), new List<Type>(){ typeof(DefaultFetchResultProcessor) } },
            { typeof(SocketIORequestTask), new List<Type>(){ typeof(DefaultFetchResultProcessor) } },
            { typeof(HttpRequestTask), new List<Type>(){ typeof(DefaultFetchResultProcessor) } }
        };

        public void AddOrUpdateFetchResultProcessors<TRequestTask>(List<Type> processorTypes) where TRequestTask : IRequestTask
        {
            var type = typeof(TRequestTask);

            if (ProviderMapper.ContainsKey(type))
            {
                ProviderMapper[type] = processorTypes;
            }
            else
            {
                ProviderMapper.Add(type, processorTypes);
            }
        }

        public void DeleteFetchResultProcessors<TRequestTask>() where TRequestTask : IRequestTask
        {
            var type = typeof(TRequestTask);

            if (ProviderMapper.ContainsKey(type))
            {
                ProviderMapper.Remove(type);
            }
        }

        public List<IFetchResultProcessor> GetFetchResultProcessors(Type requestTaskType)
        {
            if (!ProviderMapper.ContainsKey(requestTaskType))
            {
                throw new NotSupportedException($"{requestTaskType.Name} not support.");
            }

            return ProviderMapper[requestTaskType]
                .Select(t => _serviceProvider.GetService(t) as IFetchResultProcessor)
                .ToList();
        }
    }
}