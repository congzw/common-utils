using System;

namespace Common.SignalR.Refs
{
    public class DateHelper
    {
        public Func<DateTime> GetDateDefault = () => new DateTime(2000, 1, 1);
        public Func<DateTime> GetDateNow = () => DateTime.Now;
        public static DateHelper Instance = new DateHelper();
    }

    public static class DatetimeExtensions
    {
        public static string AsFormat(this DateTime datetime, string format = "yyyyMMdd-HH:mm:ss")
        {
            return datetime.ToString(format);
        }

        public static string AsFormat(this DateTime? datetime, string format = "yyyyMMdd-HH:mm:ss")
        {
            return datetime?.ToString(format);
        }
    }
}
