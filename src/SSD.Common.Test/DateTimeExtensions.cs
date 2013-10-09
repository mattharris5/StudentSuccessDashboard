using System;

namespace SSD
{
    public static class DateTimeExtensions
    {
        public static bool WithinTimeSpanOf(this DateTime value, TimeSpan timeSpan, DateTime of)
        {
            long actualDelta = Math.Abs(value.Subtract(of).Ticks);
            return actualDelta <= timeSpan.Ticks;
        }

        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }
    }
}
