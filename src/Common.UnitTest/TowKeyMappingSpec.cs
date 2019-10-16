using Common.Common.ExtTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class TowKeyMappingSpec
    {
        [TestMethod]
        public void TryGetLogicConnectionId_Should_Ok()
        {
            var towKeyMapping = new TowKeyMapping();
            towKeyMapping.SetMappingWithIdAndLogicId("A", "1");
            towKeyMapping.SetMappingWithIdAndLogicId("B", "2");

            towKeyMapping.TryGetLogicConnectionId("a").ShouldEqual("1");
            towKeyMapping.TryGetConnectionId("1").ShouldEqual("A");
            
            towKeyMapping.TryGetLogicConnectionId("B").ShouldEqual("2");
            towKeyMapping.TryGetConnectionId("2").ShouldEqual("B");

            towKeyMapping.TryGetLogicConnectionId("C").ShouldEqual(null);
            towKeyMapping.TryGetConnectionId("3").ShouldEqual(null);
        }
    }
    
    namespace Common.ExtTest
    {
        #region demo

        public static class DoubleKeyMappingExtensions
        {
            //Key1: ConnectionId
            //Key2: LogicConnectionId

            public static void SetMappingWithIdAndLogicId(this TowKeyMapping mapping, string connectionId, string logicConnectionId)
            {
                mapping?.AddMapping(connectionId, logicConnectionId);
            }

            public static string TryGetLogicConnectionId(this TowKeyMapping mapping, string connectionId)
            {
                return mapping?.TryGetKey2(connectionId);
            }

            public static string TryGetConnectionId(this TowKeyMapping mapping, string logicConnectionId)
            {
                return mapping?.TryGetKey1(logicConnectionId);
            }
        }

        #endregion
    }
}
