using Ambition.Fetcher;

namespace Ambition.Processor
{
    public interface IFetchResultProcessor
    {
        void Process(FetchResult fetchResult);
    }
}