using System;

namespace Common.Logs.Refs
{
    public class ExpiredIn
    {
        public ExpiredIn()
        {
            ExpiredInterval = TimeSpan.FromSeconds(3);
        }

        public DateTime? LastCheckAt { get; set; }
        public TimeSpan ExpiredInterval { get; set; }

        public bool ShouldExpired(DateTime now)
        {
            if (LastCheckAt == null)
            {
                LastCheckAt = now;
                return true;
            }

            var duration = now - LastCheckAt;
            if (duration >= ExpiredInterval)
            {
                LastCheckAt = now;
                return true;
            }

            return false;
        }

        public static ExpiredIn Create(TimeSpan expiredInterval)
        {
            return new ExpiredIn() { ExpiredInterval = expiredInterval };
        }
    }
}
