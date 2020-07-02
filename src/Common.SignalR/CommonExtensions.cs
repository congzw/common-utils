using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class CommonExtensions
    {
        public static bool MyEquals(this string value, string value2, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var valueFix = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                valueFix = value.Trim();
            }
            var value2Fix = string.Empty;
            if (!string.IsNullOrWhiteSpace(value2))
            {
                value2Fix= value2.Trim();
            }
            return valueFix.Equals(value2Fix, comparison);
        }

        public static bool MyContains(this IEnumerable<string> list, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var theOne = list?.FirstOrDefault(x => x.MyEquals(value, comparison));
            return theOne != null;
        }

        public static string MyFind(this IEnumerable<string> list, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var theOne = list?.FirstOrDefault(x => x.MyEquals(value, comparison));
            return theOne;
        }

        public static void MyRemove(this IList<string> list, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var theOne = list?.FirstOrDefault(x => x.MyEquals(value, comparison));
            if (theOne == null)
            {
                return;
            }
            list.Remove(theOne);
        }
    }
}
