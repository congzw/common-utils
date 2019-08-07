using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class SimpleLogSpec
    {
        [TestMethod]
        public void Log_Should_LogByLevel()
        {
            var simpleLog = SimpleLogFactory.Resolve().Create("Foo");
            simpleLog.EnabledLevel = SimpleLogLevel.Information;

            simpleLog.ShouldLog(SimpleLogLevel.Trace).ShouldFalse();
            simpleLog.ShouldLog(SimpleLogLevel.Debug).ShouldFalse();

            simpleLog.ShouldLog(SimpleLogLevel.Information).ShouldTrue();
            simpleLog.ShouldLog(SimpleLogLevel.Warning).ShouldTrue();
            simpleLog.ShouldLog(SimpleLogLevel.Error).ShouldTrue();
            simpleLog.ShouldLog(SimpleLogLevel.Critical).ShouldTrue();
        }
    }

    [TestClass]
    public class SimpleLogFactorySpec
    {
        [TestMethod]
        public void Create_Category_Null_Should_Return_DefaultLevel()
        {
            var simpleLogFactory = GetFactory();
            var simpleLog = simpleLogFactory.Create(null);

            simpleLog.ShouldNotNull();
            simpleLog.EnabledLevel.ShouldEqual(defaultLevel);
        }

        [TestMethod]
        public void Create_Category_NotSet_Should_Return_DefaultLevel()
        {
            var simpleLogFactory = GetFactory();
            var simpleLog = simpleLogFactory.Create(Guid.NewGuid().ToString());

            simpleLog.ShouldNotNull();
            simpleLog.EnabledLevel.ShouldEqual(defaultLevel);
        }

        [TestMethod]
        public void Create_Category_HasSet_Should_Return_RightLevel()
        {
            var simpleLogFactory = GetFactory();
            var simpleLog = simpleLogFactory.Create(fooCategory);

            simpleLog.ShouldNotNull();
            simpleLog.EnabledLevel.ShouldEqual(fooLevel);
        }

        private SimpleLogLevel defaultLevel = SimpleLogLevel.Debug;
        private SimpleLogLevel fooLevel = SimpleLogLevel.Error;
        private string fooCategory = "Foo";
        private ISimpleLogFactory GetFactory()
        {
            var simpleLogSettings = new SimpleLogSettings();
            simpleLogSettings.Default = new SimpleLogSetting() { Category = SimpleLogSettings.DefaultCategory, EnabledLevel = defaultLevel };
            simpleLogSettings.SetEnabledLevel(fooCategory, fooLevel);
            return new SimpleLogFactory(simpleLogSettings);
        }
    }
}
