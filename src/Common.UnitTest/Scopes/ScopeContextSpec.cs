using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Scopes
{
    [TestClass]
    public class ScopeContextSpec
    {
        [TestMethod]
        public void GetScopeContext_NotExist_ShouldOK()
        {
            ScopeContext.ReposFunc = () => new MockRepository(true);
            ScopeContext.GetScopeContext("NotExist", false).ShouldNull();
        }

        [TestMethod]
        public void GetScopeContext_Exist_ShouldOK()
        {
            ScopeContext.ReposFunc = () => new MockRepository(true);
            ScopeContext.GetScopeContext("001", false).ShouldNotNull();
        }

        [TestMethod]
        public void GetScopeContext_ReplaceReposFunc_ShouldOK()
        {
            ScopeContext.ReposFunc = () => new MockRepository(true);
            ScopeContext.GetScopeContext("001", false).ShouldNotNull();

            //replace repos
            ScopeContext.ReposFunc = () => new MockRepository(false);
            ScopeContext.GetScopeContext("001", false).ShouldNull();
        }

        [TestMethod]
        public void SetItem_NotExistKey_ShouldAdd()
        {
            ScopeContext.ReposFunc = () => new MockRepository(true);
            var scopeContext = ScopeContext.GetScopeContext("001", false);
            scopeContext.SetItem("bar", "BAR");
            scopeContext.GetItem("bar", null).ShouldEqual("BAR");
        }

        [TestMethod]
        public void SetItem_ExistKey_ShouldReplace()
        {
            ScopeContext.ReposFunc = () => new MockRepository(true);
            var scopeContext = ScopeContext.GetScopeContext("001", false);
            scopeContext.SetItem("foo", "BAR");
            scopeContext.GetItem("foo", null).ShouldEqual("BAR");
        }

        [TestMethod]
        public void GetItem_ExistKey_ShouldOK()
        {
            ScopeContext.ReposFunc = () => new MockRepository(true);
            var scopeContext = ScopeContext.GetScopeContext("001", false);
            scopeContext.GetItem("foo").ShouldEqual("FOO");
            scopeContext.GetItem("FoO").ShouldEqual("FOO");
        }

        [TestMethod]
        public void GetItem_NotExistKey_ShouldOK()
        {
            ScopeContext.ReposFunc = () => new MockRepository(true);
            var scopeContext = ScopeContext.GetScopeContext("001", false);
            scopeContext.GetItem("NotExist", null).ShouldNull();
            scopeContext.GetItem("NotExist", "A").ShouldEqual("A");
        }

        [TestMethod]
        public void GetItemAs_Convert_ShouldOK()
        {
            ScopeContext.ReposFunc = () => new MockRepository(true);
            var scopeContext = ScopeContext.GetScopeContext("001", false);
            scopeContext.GetItemAs("isFoo", false).ShouldTrue();
            scopeContext.GetItemAs("isFooX", false).ShouldFalse();
        }
    }
}
