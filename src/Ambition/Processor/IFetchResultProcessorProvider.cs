using Ambition.Scheduler;
using System;
using System.Collections.Generic;

namespace Ambition.Processor
{
    public interface IFetchResultProcessorProvider
    {
        void AddOrUpdateFetchResultProcessors<TRequestTask>(List<Type> processorTypes) where TRequestTask : IRequestTask;

        void DeleteFetchResultProcessors<TRequestTask>() where TRequestTask : IRequestTask;

        List<IFetchResultProcessor> GetFetchResultProcessors(Type requestTaskType);
    }
}