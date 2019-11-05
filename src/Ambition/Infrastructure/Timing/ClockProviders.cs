namespace Ambition.Timing
{
    public static class ClockProviders
    {
        public static LocalClockProvider Local { get; } = new LocalClockProvider();

        public static UnspecifiedClockProvider Unspecified { get; } = new UnspecifiedClockProvider();

        public static UtcClockProvider Utc { get; } = new UtcClockProvider();
    }
}