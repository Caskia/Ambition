using Ambition.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ambition.Pipeline
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

        public void AddOrUpdatePipelines<TRequestTask>(List<Type> pipelineTypes) where TRequestTask : IRequestTask
        {
            var type = typeof(TRequestTask);

            if (ProviderMapper.ContainsKey(type))
            {
                ProviderMapper[type] = pipelineTypes;
            }
            else
            {
                ProviderMapper.Add(type, pipelineTypes);
            }
        }

        public void DeletePipelines<TRequestTask>() where TRequestTask : IRequestTask
        {
            var type = typeof(TRequestTask);

            if (ProviderMapper.ContainsKey(type))
            {
                ProviderMapper.Remove(type);
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