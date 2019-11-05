using Ambition.Processor;
using System;
using System.Collections.Generic;

namespace Ambition.Scheduler
{
    public interface IRequestTask
    {
        IList<IFetchResultProcessor> FetchResultProcessors { get; set; }

        string Identity { get; }

        DateTime? LastTryTime { get; set; }

        DateTime NextTryTime { get; set; }

        int Priority { get; set; }

        ContentType ResultContentType { get; set; }

        RequestTaskStatus Status { get; set; }

        short TryCount { get; set; }

        Uri Uri { get; set; }

        IRequestTask AddFetchResultProcessor(IFetchResultProcessor fetchResultProcessor);

        DateTime? CalculateNextTryTime();
    }
}