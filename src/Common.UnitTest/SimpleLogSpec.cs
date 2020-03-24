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
        
        [TestMethod]
        public void Log_SimpleEx_Should_Ok()
        {
            var testHelper = SimpleLogTestHelper.Create("foo", SimpleLogLevel.Information, SimpleLogLevel.Information);
            var simpleLogFactory = testHelper.CreateFactory();
            var logActions = simpleLogFactory.LogActions;

            var mockLogger = new MockLogger();
            logActions["MockLog"] = new LogMessageAction("MockLog", true, args => mockLogger.Log(args));

            var simpleLog = simpleLogFactory.Create("Foo");
            simpleLog.LogInfo("ABC");
            mockLogger.Invoked.ShouldTrue();
        }

        [TestMethod]
        public void Log_SimpleEx_Should_BySettings()
        {
            var testHelper = SimpleLogTestHelper.Create("foo", SimpleLogLevel.Error, SimpleLogLevel.Information);
            var simpleLogFactory = testHelper.CreateFactory();
            var logActions = simpleLogFactory.LogActions;

            var mockLogger = new MockLogger();
            logActions["MockLog"] = new LogMessageAction("MockLog", true, args => mockLogger.Log(args));

            var simpleLog = simpleLogFactory.Create("Foo");
            simpleLog.LogInfo("ABC");
            mockLogger.Invoked.ShouldFalse();
            
            var simpleLog2 = simpleLogFactory.Create("Foo2");
            simpleLog2.LogInfo("ABC");
            mockLogger.Invoked.ShouldTrue();
        }
    }

    public class MockLogger
    {
        public bool Invoked { get; set; }

        public void Log(LogMessageArgs args)
        {
            Invoked = true;
        }
    }

    [TestClass]
    public class SimpleLogFactorySpec
    {
        [TestMethod]
        public void Create_Category_Null_Should_Return_DefaultLevel()
        {
            var simpleLogFactory = CreateFactory();
            var simpleLog = simpleLogFactory.Create(null);

            simpleLog.ShouldNotNull();
            simpleLog.EnabledLevel.ShouldEqual(defaultLevel);
        }

        [TestMethod]
        public void Create_Category_NotSet_Should_Return_DefaultLevel()
        {
            var simpleLogFactory = CreateFactory();
            var simpleLog = simpleLogFactory.Create(Guid.NewGuid().ToString());

            simpleLog.ShouldNotNull();
            simpleLog.EnabledLevel.ShouldEqual(defaultLevel);
        }

        [TestMethod]
        public void Create_Category_HasSet_Should_Return_RightLevel()
        {
            var simpleLogFactory = CreateFactory();
            var simpleLog = simpleLogFactory.Create(fooCategory);

            simpleLog.ShouldNotNull();
            simpleLog.EnabledLevel.ShouldEqual(fooLevel);
        }

        private SimpleLogTestHelper _testHelper = null;
        private SimpleLogLevel defaultLevel = SimpleLogLevel.Debug;
        private SimpleLogLevel fooLevel = SimpleLogLevel.Error;
        private string fooCategory = "Foo";
        private ISimpleLogFactory CreateFactory()
        {
            if (_testHelper == null)
            {
                _testHelper = SimpleLogTestHelper.Create(fooCategory, fooLevel, defaultLevel);
            }
            return _testHelper.CreateFactory();
        }
    }

    public class SimpleLogTestHelper
    {
        public ISimpleLogFactory CreateFactory()
        {
            var simpleLogSettings = new SimpleLogSettings();
            simpleLogSettings.Default = new SimpleLogSetting() { Category = SimpleLogSettings.DefaultCategory, EnabledLevel = DefaultLevel };
            simpleLogSettings.SetEnabledLevel(FooCategory, FooLevel);
            return new SimpleLogFactory(simpleLogSettings, new LogMessageActions());
        }
        public SimpleLogLevel DefaultLevel { get; set; }
        public SimpleLogLevel FooLevel { get; set; }
        public string FooCategory { get; set; }
        public static SimpleLogTestHelper Create(string fooCategory, SimpleLogLevel fooLevel, SimpleLogLevel defaultLevel)
        {
            var simpleLogTestHelper = new SimpleLogTestHelper();
            simpleLogTestHelper.FooCategory = fooCategory;
            simpleLogTestHelper.FooLevel = fooLevel;
            simpleLogTestHelper.DefaultLevel = defaultLevel;
            return simpleLogTestHelper;
        }
    }
}
