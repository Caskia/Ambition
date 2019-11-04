using System;
using System.Collections.Generic;

namespace Ambition.Core.Pipeline
{
    public interface IPipelineProvider
    {
        void AddOrUpdatePipelines(Type requestTaskType, List<Type> pipelineTypes);

        void DeletePipelines(Type requestTaskType);

        List<IPipeline> GetPipelines(Type requestTaskType);
    }
}