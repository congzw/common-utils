using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Scopes
{
    [TestClass]
    public class ScopeContextSpec
    {
        [TestMethod]
        public void GetScopeContext_NotExist_ShouldOK()
        {
            ScopeContext.Resolve = () => new MockScopeService(true);
            ScopeContext.GetScopeContext("NotExist", false).ShouldNull();
        }

        [TestMethod]
        public void GetScopeContext_Exist_ShouldOK()
        {
            ScopeContext.Resolve = () => new MockScopeService(true);
            ScopeContext.GetScopeContext("001", false).ShouldNotNull();
        }

        [TestMethod]
        public void GetScopeContext_ReplaceResolve_ShouldOK()
        {
            ScopeContext.Resolve = () => new MockScopeService(true);
            ScopeContext.GetScopeContext("001", false).ShouldNotNull();

            //replace repos
            ScopeContext.Resolve = () => new MockScopeService(false);
            ScopeContext.GetScopeContext("001", false).ShouldNull();
        }

        [TestMethod]
        public void SetItem_NotExistKey_ShouldAdd()
        {
            ScopeContext.Resolve = () => new MockScopeService(true);
            var scopeContext = ScopeContext.GetScopeContext("001", false);
            scopeContext.SetItem("bar", "BAR");
            scopeContext.GetItem("bar", null).ShouldEqual("BAR");
        }

        [TestMethod]
        public void SetItem_ExistKey_ShouldReplace()
        {
            ScopeContext.Resolve = () => new MockScopeService(true);
            var scopeContext = ScopeContext.GetScopeContext("001", false);
            scopeContext.SetItem("foo", "BAR");
            scopeContext.GetItem("foo", null).ShouldEqual("BAR");
        }

        [TestMethod]
        public void GetItem_ExistKey_ShouldOK()
        {
            ScopeContext.Resolve = () => new MockScopeService(true);
            var scopeContext = ScopeContext.GetScopeContext("001", false);
            scopeContext.GetItem("foo").ShouldEqual("FOO");
            scopeContext.GetItem("FoO").ShouldEqual("FOO");
        }

        [TestMethod]
        public void GetItem_NotExistKey_ShouldOK()
        {
            ScopeContext.Resolve = () => new MockScopeService(true);
            var scopeContext = ScopeContext.GetScopeContext("001", false);
            scopeContext.GetItem("NotExist", null).ShouldNull();
            scopeContext.GetItem("NotExist", "A").ShouldEqual("A");
        }

        [TestMethod]
        public void GetItemAs_Convert_ShouldOK()
        {
            ScopeContext.Resolve = () => new MockScopeService(true);
            var scopeContext = ScopeContext.GetScopeContext("001", false);
            scopeContext.GetItemAs("isFoo", false).ShouldTrue();
            scopeContext.GetItemAs("isFooX", false).ShouldFalse();
        }
    }
}
