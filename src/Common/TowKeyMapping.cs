using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

// ReSharper disable CheckNamespace
namespace Common
{
    public class TowKeyMapping
    {
        public IDictionary<string, string> Dic12 { get; set; }

        public IDictionary<string, string> Dic21 { get; set; }

        public TowKeyMapping()
        {
            Dic12 = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Dic21 = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        
        public void AddMapping(string key1, string key2)
        {
            Dic12[key1] = key2;
            Dic21[key2] = key1;
        }

        public string TryGetKey1(string key2)
        {
            Dic21.TryGetValue(key2, out var key1);
            return key1;
        }

        public string TryGetKey2(string key1)
        {
            Dic12.TryGetValue(key1, out var key2);
            return key2;
        }
    }

    #region demo

    //public static class DoubleKeyMappingExtensions
    //{
    //    //Key1: ConnectionId
    //    //Key2: LogicConnectionId

    //    public static void SetMappingWithIdAndLogicId(this TowKeyMapping mapping, string connectionId, string logicConnectionId)
    //    {
    //        mapping?.AddMapping(connectionId, logicConnectionId);
    //    }

    //    public static string TryGetLogicConnectionId(this TowKeyMapping mapping, string connectionId)
    //    {
    //        return mapping?.TryGetKey2(connectionId);
    //    }

    //    public static string TryGetConnectionId(this TowKeyMapping mapping, string logicConnectionId)
    //    {
    //        return mapping?.TryGetKey1(logicConnectionId);
    //    }
    //}
    #endregion
}
