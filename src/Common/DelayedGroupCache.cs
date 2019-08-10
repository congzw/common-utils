using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class DelayedGroupCache<T>
    {
        protected readonly object Lock = new object();

        public DelayedGroupCache()
        {
            DelayedGroups = new ConcurrentDictionary<string, DelayedGroup<T>>(StringComparer.OrdinalIgnoreCase);
            DelaySpan = TimeSpan.FromSeconds(10);
        }

        public TimeSpan DelaySpan { get; set; }

        public IDictionary<string, DelayedGroup<T>> DelayedGroups { get; set; }
        
        public void AppendToGroups(IList<T> items, Func<T, string> getGroupKey, Func<T, DateTime> getAppendAt)
        {
            if (getGroupKey == null)
            {
                throw new ArgumentNullException(nameof(getGroupKey));
            }

            if (getAppendAt == null)
            {
                throw new ArgumentNullException(nameof(getAppendAt));
            }

            if (items == null || items.Count == 0)
            {
                return;
            }

            lock (Lock)
            {
                var groups = items.GroupBy(getGroupKey);
                foreach (var itemGroup in groups)
                {
                    var groupKey = itemGroup.Key;
                    var groupItems = itemGroup.ToList();
                    if (groupItems.Count > 0)
                    {
                        var lastItemDate = groupItems.Max(getAppendAt);
                        DelayedGroups.TryGetValue(groupKey, out var theGroup);
                        if (theGroup == null)
                        {
                            theGroup = DelayedGroup<T>.Create(groupKey, lastItemDate);
                            DelayedGroups.Add(groupKey, theGroup);
                        }
                        theGroup.LastItemDate = lastItemDate;
                        theGroup.AppendItems(groupItems);
                    }
                }
            }
        }
        
        public IList<DelayedGroup<T>> PopExpiredGroups(DateTime popAt)
        {
            lock (Lock)
            {
                var expiredGroups = DelayedGroups.Where(x => x.Value.LastItemDate.Add(DelaySpan) <= popAt).ToList();
                foreach (var expiredGroup in expiredGroups)
                {
                    DelayedGroups.Remove(expiredGroup);
                }
                return expiredGroups.Select(x => x.Value).ToList();
            }
        }
    }

    public class DelayedGroup<T>
    {
        public DelayedGroup()
        {
            Items = new List<T>();
        }

        public string GroupKey { get; set; }
        public DateTime LastItemDate { get; set; }
        public IList<T> Items { get; set; }

        #region for easy use

        public DelayedGroup<T> AppendItem(T item)
        {
            if (!Items.Contains(item))
            {
                Items.Add(item);
            }
            return this;
        }
        public DelayedGroup<T> AppendItems(IEnumerable<T> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    AppendItem(item);
                }
            }
            return this;
        }
        public static DelayedGroup<T> Create(string groupKey, DateTime createAt)
        {
            var theGroup = new DelayedGroup<T>();
            theGroup.GroupKey = groupKey;
            theGroup.LastItemDate = createAt;
            return theGroup;
        }
        

        #endregion
    }
}
