using Ambition.Core.Scheduler;
using System.Collections.Generic;

namespace Ambition.Core.Processor
{
    public interface IFetchResultProcessorsProvider
    {
        List<IFetchResultProcessor> GetFetchResultProcessors(IRequestTask requestTask);
    }
}