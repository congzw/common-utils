using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public enum OverlapsInclude
    {
        None = 0,
        Begin = 0x01,
        End = 0x10,
        Both = 0x11
    }

    public interface IDateTimeRange
    {
        DateTime Begin { get; set; }
        DateTime End { get; set; }
    }

    public class DateTimeRange : IDateTimeRange
    {
        protected DateTimeRange() { }
        public DateTimeRange(DateTime begin, DateTime end)
        {
            Begin = begin;
            End = end;
        }
        public DateTimeRange(DateTime begin, TimeSpan duration)
        {
            Begin = begin;
            End = begin.Add(duration);
        }

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        public DateTimeRange NewEnd(DateTime newEnd)
        {
            return new DateTimeRange(this.Begin, newEnd);
        }
        public DateTimeRange NewDuration(TimeSpan newDuration)
        {
            return new DateTimeRange(this.Begin, newDuration);
        }
        public DateTimeRange NewBegin(DateTime newBegin)
        {
            return new DateTimeRange(newBegin, this.End);
        }

        public static DateTimeRange Create(DateTime begin, DateTime end)
        {
            return new DateTimeRange(begin, end);
        }
    }

    public static class DateRangeExtensions
    {
        public static bool Overlaps<TSource>(this TSource dateTimeRange, IDateTimeRange another, OverlapsInclude overlapsInclude = OverlapsInclude.None) where TSource : IDateTimeRange
        {
            return dateTimeRange.Overlaps(another.Begin, another.End, overlapsInclude);
        }

        public static bool Overlaps<TSource>(this TSource dateTimeRange, DateTime begin, DateTime? end, OverlapsInclude overlapsInclude = OverlapsInclude.None) where TSource : IDateTimeRange
        {
            var endValue = end ?? DateTime.MaxValue;
            switch (overlapsInclude)
            {
                case OverlapsInclude.None:
                    return dateTimeRange.Begin < endValue && dateTimeRange.End > begin;
                case OverlapsInclude.Begin:
                    return dateTimeRange.Begin <= endValue && dateTimeRange.End > begin;
                case OverlapsInclude.End:
                    return dateTimeRange.Begin < endValue && dateTimeRange.End >= begin;
                case OverlapsInclude.Both:
                    return dateTimeRange.Begin <= endValue && dateTimeRange.End >= begin;
                default:
                    throw new ArgumentOutOfRangeException(nameof(overlapsInclude), overlapsInclude, null);
            }
        }

        public static IEnumerable<TSource> FilterOverlaps<TSource>(this IEnumerable<TSource> dateTimeRanges,
            DateTime begin, DateTime? end, OverlapsInclude overlapsInclude = OverlapsInclude.None) where TSource : IDateTimeRange
        {
            return dateTimeRanges.Where(x => x.Overlaps(begin, end));
        }
    }

    #region DateTimeRangeArchive

    public interface IDateTimeRangeArchiveParse
    {
        DateTimeRangeArchive CreateArchive(DateTime createAt);
        DateTimeRangeArchive TryParseArchive(string archiveId);
    }

    public class DateTimeRangeArchive : IDateTimeRange
    {
        public string ArchiveId { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        public override string ToString()
        {
            return string.Format("{0} => {1} ~ {2}", ArchiveId, Begin, End);
        }

        #region for simple use
        
        public static DateTimeRangeArchive Create(DateTime createAt, IDateTimeRangeArchiveParse parser = null)
        {
            var theParser = parser ?? ResolveParse();
            return theParser.CreateArchive(createAt);
        }

        public static DateTimeRangeArchive TryParse(string archiveId, IDateTimeRangeArchiveParse parser = null)
        {
            var theParser = parser ?? ResolveParse();
            return theParser.TryParseArchive(archiveId);
        }

        public static IEnumerable<DateTimeRangeArchive> TryParse(IEnumerable<string> archiveIds, IDateTimeRangeArchiveParse parser = null)
        {
            foreach (var archiveId in archiveIds)
            {
                var archive = TryParse(archiveId, parser);
                if (archive != null)
                {
                    yield return archive;
                }
            }
        }


        #endregion

        #region for di extensions

        private static readonly Lazy<IDateTimeRangeArchiveParse> Instance = new Lazy<IDateTimeRangeArchiveParse>(() => new HourRangeArchiveParse());
        public static Func<IDateTimeRangeArchiveParse> ResolveParse { get; set; } = () => Instance.Value;

        #endregion
    }

    public class HourRangeArchiveParse : IDateTimeRangeArchiveParse
    {
        public HourRangeArchiveParse()
        {
            DateFormat = "yyyyMMddHH";
        }

        public string DateFormat { get; set; }

        public DateTimeRangeArchive CreateArchive(DateTime createAt)
        {
            var dateTimeRangeArchive = new DateTimeRangeArchive();
            var hourDate = createAt.GetHourDate();
            dateTimeRangeArchive.Begin = hourDate;
            dateTimeRangeArchive.End = hourDate.AddHours(1).AddMilliseconds(-1);
            dateTimeRangeArchive.ArchiveId = hourDate.ToString(DateFormat);
            return dateTimeRangeArchive;
        }

        public DateTimeRangeArchive TryParseArchive(string archiveId)
        {
            var dateTimeRangeArchive = new DateTimeRangeArchive();

            var beginDate = archiveId.TryParseDate(DateFormat);
            if (beginDate == null)
            {
                return null;
            }

            dateTimeRangeArchive.Begin = beginDate.Value;
            dateTimeRangeArchive.End = beginDate.Value.AddHours(1).AddMilliseconds(-1);
            dateTimeRangeArchive.ArchiveId = archiveId;
            return dateTimeRangeArchive;
        }
    }

    public class DayRangeArchiveParse : IDateTimeRangeArchiveParse
    {
        public DayRangeArchiveParse()
        {
            DateFormat = "yyyyMMdd";
        }

        public string DateFormat { get; set; }
        public DateTimeRangeArchive CreateArchive(DateTime createAt)
        {
            var dateTimeRangeArchive = new DateTimeRangeArchive();
            var dayDate = createAt.GetDayDate();
            dateTimeRangeArchive.Begin = dayDate;
            dateTimeRangeArchive.End = dayDate.AddDays(1).AddMilliseconds(-1); ;
            dateTimeRangeArchive.ArchiveId = dayDate.ToString(DateFormat);
            return dateTimeRangeArchive;
        }

        public DateTimeRangeArchive TryParseArchive(string archiveId)
        {
            var dateTimeRangeArchive = new DateTimeRangeArchive();

            var beginDate = archiveId.TryParseDate(DateFormat);
            if (beginDate == null)
            {
                return null;
            }

            dateTimeRangeArchive.Begin = beginDate.Value;
            dateTimeRangeArchive.End = beginDate.Value.AddDays(1).AddMilliseconds(-1);
            dateTimeRangeArchive.ArchiveId = archiveId;
            return dateTimeRangeArchive;
        }
    }

    #endregion
}
