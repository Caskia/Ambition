using Ambition.Core.Fetcher;

namespace Ambition.Core.Processor
{
    public interface IFetchResultProcessor
    {
        void Process(FetchResult fetchResult);
    }
}