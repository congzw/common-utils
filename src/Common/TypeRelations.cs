// ReSharper disable CheckNamespace

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common
{
    public class TypeRelations
    {
        //for auto fix multi type
        public IDictionary<Type, Type> Mappings { get; set; }

        public TypeRelations()
        {
            Mappings = new ConcurrentDictionary<Type, Type>();
        }

        public void Register(Type baseType, params Type[] subTypes)
        {
            foreach (var subType in subTypes)
            {
                Mappings[subType] = baseType;
            }
        }
        
        public Type TryGetBaseType(Type subType, bool returnSameTypeIfNotFind = true)
        {
            var baseType = returnSameTypeIfNotFind ? subType : null;
            return Mappings.ContainsKey(subType) ? Mappings[subType] : baseType;
        }

        public static TypeRelations Instance = new TypeRelations();
    }
}
