using System;

namespace Ambition.Scheduler
{
    public interface IRequestTask
    {
        string Identity { get; }

        DateTime? LastTryTime { get; set; }

        DateTime NextTryTime { get; set; }

        int Priority { get; set; }

        ContentType ResultContentType { get; set; }

        RequestTaskStatus Status { get; set; }

        short TryCount { get; set; }

        Uri Uri { get; set; }

        DateTime CalculateNextTryTime();
    }
}