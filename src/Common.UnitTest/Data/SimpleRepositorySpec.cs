using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Data
{
    [TestClass]
    public class SimpleRepositorySpec
    {
        [TestMethod]
        public void Query_NotInit_ShouldEmpty()
        {
            var memoryRepository = CreateRepository(false);
            memoryRepository.Query<MockUser>().Count().ShouldEqual(0);
        }

        [TestMethod]
        public void Query_Init_ShouldOk()
        {
            var memoryRepository = CreateRepository(true);
            memoryRepository.Query<MockUser>().Count().ShouldEqual(3);
            memoryRepository.Query<MockRole>().Count().ShouldEqual(2);
        }

        [TestMethod]
        public void Get_Exist_ShouldOk()
        {
            var memoryRepository = CreateRepository(true);
            memoryRepository.Get<MockUser>(1).ShouldNotNull();
            memoryRepository.Get<MockUser>(100).ShouldNull();
        }

        [TestMethod]
        public void Add_Exist_ShouldThrows()
        {
            var memoryRepository = CreateRepository(true);
            AssertHelper.ShouldThrows<Exception>(() =>
            {
                memoryRepository.Add(new MockUser() { Id = 1, UserName = "BAD" });
            });
        }

        [TestMethod]
        public void Add_New_ShouldOK()
        {
            var memoryRepository = CreateRepository(true);
            var count = memoryRepository.Query<MockUser>().Count();
            memoryRepository.Add(new MockUser() { Id = 5, UserName = "User5" });
            memoryRepository.Query<MockUser>().Count().ShouldEqual(count + 1);
        }

        [TestMethod]
        public void Update_NotExist_ShouldThrows()
        {
            var memoryRepository = CreateRepository(true);
            AssertHelper.ShouldThrows<Exception>(() =>
            {
                memoryRepository.Update(new MockUser() { Id = 5, UserName = "UserNew" });
            });
        }

        [TestMethod]
        public void Update_Exist_ShouldOK()
        {
            var memoryRepository = CreateRepository(true);
            memoryRepository.Update(new MockUser() { Id = 1, UserName = "UserNew" });
            memoryRepository.Get<MockUser>(1).UserName.ShouldEqual("UserNew");
        }

        [TestMethod]
        public void Delete_NotExist_ShouldOk()
        {
            var memoryRepository = CreateRepository(true);
            memoryRepository.Delete(new MockUser() { Id = 5, UserName = "UserNew" });
        }

        [TestMethod]
        public void Delete_Exist_ShouldOk()
        {
            var memoryRepository = CreateRepository(true);
            var count = memoryRepository.Query<MockUser>().Count();
            memoryRepository.Delete(new MockUser() { Id = 1, UserName = "UserNew" });
            memoryRepository.Query<MockUser>().Count().ShouldEqual(count - 1);
        }

        [TestMethod]
        public void Truncate_Exist_ShouldOk()
        {
            var memoryRepository = CreateRepository(true);
            memoryRepository.Truncate<MockUser>();
            memoryRepository.Query<MockUser>().Count().ShouldEqual(0);
        }
        
        [TestMethod]
        public void Query_TypeRelations_ShouldOk()
        {
            var memoryRepository = CreateRepository(true);

            memoryRepository.Add(new MockBoy() { Id = 1 });
            memoryRepository.Add(new MockGirl() { Id = 2 });
            memoryRepository.Query<MockBoy>().Count().ShouldEqual(1);
            memoryRepository.Query<MockGirl>().Count().ShouldEqual(1);
            memoryRepository.Query<MockKid>().Count().ShouldEqual(2);

            memoryRepository.Get<MockKid>(1).ShouldNotNull();
            memoryRepository.Get<MockBoy>(1).ShouldNotNull();
            memoryRepository.Get<MockGirl>(1).ShouldNull();

        }

        private ISimpleRepository CreateRepository(bool init)
        {
            var memoryRepository = new SimpleMemoryRepository();
            
            var map = new BaseSubTypeRelationMap();
            map.Register(typeof(MockKid), typeof(MockBoy));
            map.Register(typeof(MockKid), typeof(MockGirl));
            memoryRepository.Relations = map;

            if (init)
            {
                var userIds = new[] { 1, 2, 3 };
                var users = new List<MockUser>();
                for (int i = 0; i < 3; i++)
                {
                    users.Add(new MockUser() { UserName = "User" + i, Id = userIds[i] });
                }
                memoryRepository.Init(users);

                var roleIds = new[] { 1, 2 };
                var roles = new List<MockRole>();
                for (int i = 0; i < 2; i++)
                {
                    roles.Add(new MockRole() { Name = "Role" + i, Id = roleIds[i] });
                }
                memoryRepository.Init(roles);
            }
            return memoryRepository;
        }

        #region mocks
        
        public abstract class EntityBaseInt<T> : SimpleEntityBase<int> where T : EntityBaseInt<T>
        {
        }

        public abstract class EntityBaseInt : EntityBaseInt<EntityBaseInt>
        {
        }


        public class MockUser : EntityBaseInt
        {
            public string UserName { get; set; }
        }

        public class MockRole : EntityBaseInt
        {
            public string Name { get; set; }
        }

        public abstract class MockKid : EntityBaseInt
        {
        }
        public class MockBoy : MockKid
        {

        }
        public class MockGirl : MockKid
        {

        }

        #endregion
    }

}
