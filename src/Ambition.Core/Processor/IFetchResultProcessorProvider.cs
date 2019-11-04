using System;
using System.Collections.Generic;

namespace Ambition.Core.Processor
{
    public interface IFetchResultProcessorProvider
    {
        void AddOrUpdateFetchResultProcessors(Type requestTaskType, List<Type> processorTypes);

        void DeleteFetchResultProcessors(Type requestTaskType);

        List<IFetchResultProcessor> GetFetchResultProcessors(Type requestTaskType);
    }
}