using Ambition.Core.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ambition.Core.Processor
{
    public class DefaultFetchResultProcessorsProvider : IFetchResultProcessorsProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultFetchResultProcessorsProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Dictionary<Type, List<Type>> ProviderMapper => new Dictionary<Type, List<Type>>()
        {
            { typeof(WebSocketRequestTask), new List<Type>(){ typeof(DefaultFetchResultProcessor) } },
            { typeof(SocketIORequestTask), new List<Type>(){ typeof(DefaultFetchResultProcessor) } },
            { typeof(HttpRequestTask), new List<Type>(){ typeof(DefaultFetchResultProcessor) } }
        };

        public List<IFetchResultProcessor> GetFetchResultProcessors(IRequestTask requestTask)
        {
            var requestTaskType = requestTask.GetType();

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