using System;

namespace Common.Logs.Refs
{
    public class CheckIfNotOkAndExpired
    {
        public CheckIfNotOkAndExpired()
        {
            ExpiredIn.Create(TimeSpan.FromSeconds(3));
        }

        public virtual bool NeedCheck(DateTime now)
        {
            if (StatusOk)
            {
                return false;
            }
            return ExpiredIn.ShouldExpired(now);
        }

        public bool CheckIfNecessary(DateTime now, Func<bool> checkStatusOkFunc)
        {
            //check only necessary
            var needCheck = NeedCheck(now);
            if (!needCheck)
            {
                return StatusOk;
            }

            StatusOk = checkStatusOkFunc();
            return StatusOk;
        }

        public bool StatusOk { get; set; }

        public ExpiredIn ExpiredIn { get; set; }

        public static CheckIfNotOkAndExpired Create(TimeSpan? timeSpan = null)
        {
            return new CheckIfNotOkAndExpired() { ExpiredIn = ExpiredIn.Create(timeSpan ?? TimeSpan.FromSeconds(3)) };
        }
    }
}