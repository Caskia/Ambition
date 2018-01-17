namespace Ambition.Core
{
    public interface ISpider
    {
        void Continue();

        void Pause();

        void Start();

        void Stop();
    }
}