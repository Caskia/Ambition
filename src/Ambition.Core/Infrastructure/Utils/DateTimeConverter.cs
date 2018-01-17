using Ambition.Core.Timing;
using System;

namespace Ambition.Core.Utils
{
    public static class DateTimeConverter
    {
        public static DateTime ConvertFromTimestampMilliseconds(double timestamp)
        {
            DateTime converted = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            DateTime newDateTime = converted.AddMilliseconds(timestamp);

            return newDateTime;
        }

        public static DateTime ConvertFromTimestampSeconds(double timestamp)
        {
            DateTime converted = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            DateTime newDateTime = converted.AddSeconds(timestamp);

            return newDateTime;
        }

        public static double ConvertToTimestampMilliseconds(DateTime dateTimeUtc)
        {
            TimeSpan span = dateTimeUtc - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return span.TotalMilliseconds;
        }

        public static double ConvertToTimestampSeconds(DateTime dateTimeUtc)
        {
            TimeSpan span = dateTimeUtc - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return span.TotalSeconds;
        }

        public static TimeSpan ConvertToTimezoneOffset(string windowsTimeZoneId)
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
            return tzi.GetUtcOffset(Clock.Now);
        }
    }
}