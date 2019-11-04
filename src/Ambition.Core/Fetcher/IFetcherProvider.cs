using System;

namespace Ambition.Core.Fetcher
{
    public interface IFetcherProvider
    {
        void AddOrUpdateFetcher(Type requestTaskType, Type fetcherType);

        void DeleteFetcher(Type requestTaskType);

        IFetcher GetFetcher(Type requestTaskType);
    }
}