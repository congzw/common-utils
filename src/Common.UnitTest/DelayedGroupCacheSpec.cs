using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class DelayedGroupCacheSpec
    {
        private readonly DateTime _mockNow = new DateTime(2000, 1, 1, 0, 0, 0);

        [TestMethod]
        public void AppendToGroups_ItemsNullOrEmpty_Should_NoEffect()
        {
            var delayedGroupCache = new DelayedGroupCache<MockItem>();
            delayedGroupCache.AppendToGroups(null, item => item.GroupKey, item => item.CreateAt);
        }

        [TestMethod]
        public void AppendToGroups_GetGroupKeyNull_Should_Throws()
        {
            AssertHelper.ShouldThrows<ArgumentNullException>(() =>
            {
                var delayedGroupCache = new DelayedGroupCache<MockItem>();
                delayedGroupCache.AppendToGroups(new List<MockItem>(), null, item => item.CreateAt);
            });

            AssertHelper.ShouldThrows<ArgumentNullException>(() =>
            {
                var delayedGroupCache = new DelayedGroupCache<MockItem>();
                delayedGroupCache.AppendToGroups(new List<MockItem>(), item => item.GroupKey, null);
            });
        }

        [TestMethod]
        public void AppendToGroups_ComputeGroups_Should_Ok()
        {
            var delayedGroupCache = new DelayedGroupCache<MockItem>();
            var groupCount = 5;
            var itemCount = 3;
            var spanSecond = 1;
            var startAt = _mockNow;
            //0:00,0:01,0:02
            //     0:01,0:02,0:03            
            //          0:02,0:03,0:04      
            //               0:03,0:04,0:05      
            //                    0:04,0:05,0:06
            var mockCommands = CreateGroupCommands(groupCount, itemCount, spanSecond, startAt);
            delayedGroupCache.AppendToGroups(mockCommands, item => item.GroupKey, item => item.CreateAt);
            var delayedGroups = delayedGroupCache.DelayedGroups.Values.ToList();
            CheckGroups(delayedGroups, itemCount, groupCount);
        }
        
        [TestMethod]
        public void PopExpiredGroups_NoExpiredGroups_Should_Return_Empty()
        {
            var delayedGroupCache = new DelayedGroupCache<MockItem>();
            var delaySpan = TimeSpan.FromSeconds(5);
            var groupCount = 5;
            var itemCount = 3;
            var spanSecond = 1;
            var startAt = _mockNow;
            //0:00,0:01,0:02
            //     0:01,0:02,0:03            
            //          0:02,0:03,0:04      
            //               0:03,0:04,0:05      
            //                    0:04,0:05,0:06
            //pop at: [0:06]
            //expired groups: early than [0:06] - 5 = [0.01] => no group should return)

            delayedGroupCache.DelaySpan = delaySpan;
            var mockCommands = CreateGroupCommands(groupCount, itemCount, spanSecond, startAt);
            delayedGroupCache.AppendToGroups(mockCommands, item => item.GroupKey, item => item.CreateAt);
            var popAt = mockCommands.Max(x => x.CreateAt);
            var popExpiredGroups = delayedGroupCache.PopExpiredGroups(popAt);
            ShowCache(delayedGroupCache);
            CheckGroups(popExpiredGroups, itemCount, 0);
        }
        
        [TestMethod]
        public void PopExpiredGroups_ExpiredGroups_Should_Return()
        {
            var delayedGroupCache = new DelayedGroupCache<MockItem>();
            var delaySpan = TimeSpan.FromSeconds(2);
            var groupCount = 5;
            var itemCount = 3;
            var spanSecond = 1;
            var startAt = _mockNow;
            //0:00,0:01,0:02
            //     0:01,0:02,0:03            
            //          0:02,0:03,0:04      
            //               0:03,0:04,0:05      
            //                    0:04,0:05,0:06
            //pop at: [0:06]
            //expired groups: early than [0:06] - 2 = [0.04] => group 0,1,2 should return

            delayedGroupCache.DelaySpan = delaySpan;
            var mockCommands = CreateGroupCommands(groupCount, itemCount, spanSecond, startAt);
            delayedGroupCache.AppendToGroups(mockCommands, item => item.GroupKey, item => item.CreateAt);
            var popAt = mockCommands.Max(x => x.CreateAt);
            var popExpiredGroups = delayedGroupCache.PopExpiredGroups(popAt);
            ShowCache(delayedGroupCache);
            CheckGroups(popExpiredGroups, itemCount, 3);
        }

        [TestMethod]
        public void PopExpiredGroups_ExpiredGroupsAutoGuessDate_Should_Return()
        {
            var delayedGroupCache = new DelayedGroupCache<MockItem>();
            var delaySpan = TimeSpan.FromSeconds(2);
            var groupCount = 5;
            var itemCount = 3;
            var spanSecond = 1;
            var startAt = _mockNow;
            //0:00,0:01,0:02
            //     0:01,0:02,0:03            
            //          0:02,0:03,0:04      
            //               0:03,0:04,0:05      
            //                    0:04,0:05,0:06
            //pop at: [0:06]
            //expired groups: early than [0:06] - 2 = [0.04] => group 0,1,2 should return

            delayedGroupCache.DelaySpan = delaySpan;
            var mockCommands = CreateGroupCommands(groupCount, itemCount, spanSecond, startAt);
            delayedGroupCache.AppendToGroups(mockCommands, item => item.GroupKey, item => item.CreateAt);
            //var popAt = mockCommands.Max(x => x.CreateAt);
            var popExpiredGroups = delayedGroupCache.PopExpiredGroups(null);
            ShowCache(delayedGroupCache);
            CheckGroups(popExpiredGroups, itemCount, 3);
        }
        [TestMethod]
        public void PopExpiredGroups_ExpiredGroupsAutoGuessDate_Empty_Should_Return_Empty()
        {
            var delayedGroupCache = new DelayedGroupCache<MockItem>();
            var delaySpan = TimeSpan.FromSeconds(2);

            delayedGroupCache.DelaySpan = delaySpan;
            //var popAt = mockCommands.Max(x => x.CreateAt);
            var popExpiredGroups = delayedGroupCache.PopExpiredGroups(null);
            ShowCache(delayedGroupCache);
            CheckGroups(popExpiredGroups, 0, 0);
        }


        [TestMethod]
        public void AppendToGroups_SubClass_Should_Ok()
        {
            var delayedGroupCache = new DelayedMockItemGroupCache();
            var delaySpan = TimeSpan.FromSeconds(2);
            var groupCount = 5;
            var itemCount = 3;
            var spanSecond = 1;
            var startAt = _mockNow;
            //0:00,0:01,0:02
            //     0:01,0:02,0:03            
            //          0:02,0:03,0:04      
            //               0:03,0:04,0:05      
            //                    0:04,0:05,0:06
            //pop at: [0:06]
            //expired groups: early than [0:06] - 2 = [0.04] => group 0,1,2 should return

            delayedGroupCache.DelaySpan = delaySpan;
            var mockCommands = CreateGroupCommands(groupCount, itemCount, spanSecond, startAt);
            delayedGroupCache.AppendToGroups(mockCommands);
            var popAt = mockCommands.Max(x => x.CreateAt);
            var popExpiredGroups = delayedGroupCache.PopExpiredGroups(popAt);
            ShowCache(delayedGroupCache);
            CheckGroups(popExpiredGroups, itemCount, 3);
        }
        
        private void CheckGroups(IList<DelayedGroup<MockItem>> delayedGroups, int itemCount, int groupCount)
        {
            string.Format("====check group count: {0} should equal: {1}====", delayedGroups.Count, groupCount).Log();
            foreach (var delayedGroup in delayedGroups)
            {
                string.Format("----{0} last item date: {1:yyyy-MM-dd HH:ss:mm}----", delayedGroup.GroupKey, delayedGroup.LastItemDate).Log();
                delayedGroup.Items.Log();
                delayedGroup.Items.Count.ShouldEqual(itemCount);
            }
            delayedGroups.Count.ShouldEqual(groupCount);
        }

        private void ShowCache(DelayedGroupCache<MockItem> cache)
        {
            "-------cache info--------".Log();
            foreach (var delayedGroup in cache.DelayedGroups.Values)
            {
                string.Format("{0} count: {1}, LastItemDate: {2:yyyy-MM-dd HH:ss:mm}", delayedGroup.GroupKey, delayedGroup.Items.Count, delayedGroup.LastItemDate).Log();
            }
            "-------------------------".Log();
            "".Log();
        }

        private IList<MockItem> CreateGroupCommands(int groupCount, int itemCount, int spanSecond, DateTime startAt)
        {
            var mockCommands = new List<MockItem>();
            for (int i = 0; i < groupCount; i++)
            {
                var commands = Create("group" + i, itemCount, startAt.AddSeconds(i * spanSecond), spanSecond);
                mockCommands.AddRange(commands);
            }
            return mockCommands;
        }

        private IList<MockItem> Create(string groupKey, int itemCount, DateTime startAt, int spanSecond)
        {
            var items = new List<MockItem>();
            for (int i = 0; i < itemCount; i++)
            {
                var item = new MockItem();
                item.GroupKey = groupKey;
                item.Id = groupKey + "_" + i;
                item.CreateAt = startAt.AddSeconds(i * spanSecond);
                items.Add(item);
            }
            return items;
        }
    }

    public class MockItem
    {
        public string GroupKey { get; set; }
        public string Id { get; set; }
        public DateTime CreateAt { get; set; }

        public override string ToString()
        {
            return string.Format("{0} Item {1} at {2:yyyy-MM-dd HH:mm:ss}", this.GroupKey, this.Id, this.CreateAt);
        }
    }

    public class DelayedMockItemGroupCache : DelayedGroupCache<MockItem>
    {
        public void AppendToGroups(IList<MockItem> items)
        {
            AppendToGroups(items, item => item.GroupKey, item => item.CreateAt);
        }
    }
}
