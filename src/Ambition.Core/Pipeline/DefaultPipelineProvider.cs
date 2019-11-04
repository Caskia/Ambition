using Ambition.Core.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ambition.Core.Pipeline
{
    public class DefaultPipelineProvider : IPipelineProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultPipelineProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Dictionary<Type, List<Type>> ProviderMapper { get; } = new Dictionary<Type, List<Type>>()
        {
            { typeof(WebSocketRequestTask), new List<Type>(){ typeof(ConsolePipeline) } },
            { typeof(SocketIORequestTask), new List<Type>(){ typeof(ConsolePipeline) } },
            { typeof(HttpRequestTask), new List<Type>(){ typeof(ConsolePipeline) } }
        };

        public void AddOrUpdatePipelines(Type requestTaskType, List<Type> pipelineTypes)
        {
            if (ProviderMapper.ContainsKey(requestTaskType))
            {
                ProviderMapper[requestTaskType] = pipelineTypes;
            }
            else
            {
                ProviderMapper.Add(requestTaskType, pipelineTypes);
            }
        }

        public void DeletePipelines(Type requestTaskType)
        {
            if (ProviderMapper.ContainsKey(requestTaskType))
            {
                ProviderMapper.Remove(requestTaskType);
            }
        }

        public virtual List<IPipeline> GetPipelines(Type requestTaskType)
        {
            if (!ProviderMapper.ContainsKey(requestTaskType))
            {
                throw new NotSupportedException($"{requestTaskType.Name} not support.");
            }

            return ProviderMapper[requestTaskType]
                .Select(t => _serviceProvider.GetService(t) as IPipeline)
                .ToList();
        }
    }
}