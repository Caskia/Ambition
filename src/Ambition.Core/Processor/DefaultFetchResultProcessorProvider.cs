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

        public void AddOrUpdateFetchResultProcessors(Type requestTaskType, List<Type> processorTypes)
        {
            if (ProviderMapper.ContainsKey(requestTaskType))
            {
                ProviderMapper[requestTaskType] = processorTypes;
            }
            else
            {
                ProviderMapper.Add(requestTaskType, processorTypes);
            }
        }

        public void DeleteFetchResultProcessors(Type requestTaskType)
        {
            if (ProviderMapper.ContainsKey(requestTaskType))
            {
                ProviderMapper.Remove(requestTaskType);
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