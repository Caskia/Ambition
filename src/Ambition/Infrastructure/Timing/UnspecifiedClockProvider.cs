using System;

namespace Ambition.Timing
{
    public class UnspecifiedClockProvider : IClockProvider
    {
        internal UnspecifiedClockProvider()
        {
        }

        public DateTimeKind Kind => DateTimeKind.Unspecified;
        public DateTime Now => DateTime.Now;
        public bool SupportsMultipleTimezone => false;

        public DateTime Normalize(DateTime dateTime)
        {
            return dateTime;
        }
    }
}