using Ambition.Core.Scheduler;
using System;

namespace Ambition.Core.Fetcher
{
    public interface IFetcherProvider
    {
        void AddOrUpdateFetcher<TRequestTask>(Type fetcherType) where TRequestTask : IRequestTask;

        void DeleteFetcher<TRequestTask>() where TRequestTask : IRequestTask;

        IFetcher GetFetcher(Type requestTaskType);
    }
}