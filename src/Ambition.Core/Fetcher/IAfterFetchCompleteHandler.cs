namespace Ambition.Core.Fetcher
{
    public interface IAfterFetchCompleteHandler
    {
        void Process(FetchResult fetchResult);
    }
}