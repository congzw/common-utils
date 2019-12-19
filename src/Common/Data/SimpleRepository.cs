using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;

// 20191219: V 0.2.0, first release
// 20191218: V 0.1.0, first release
// ReSharper disable CheckNamespace
namespace Common.Data
{
    public interface ISimpleRepository
    {
        IEnumerable<T> Query<T>() where T : class;
        T Get<T>(Expression<Func<T, bool>> predicate) where T : class;
        void Add<T>(params T[] entities) where T : class;
        void Update<T>(params T[] entities) where T : class;
        void Delete<T>(params T[] entities) where T : class;
    }
    
    public static class SimpleReposExtensions
    {
        //public static object TryGetIdValue(this object model, string propName = "Id")
        //{
        //    //x => x.{propName}
        //    var paramExpression = Expression.Parameter(model.GetType());
        //    var memberExpression = CreatePropertyExpression(paramExpression, propName);
        //    var propertyInfo = memberExpression.Member as PropertyInfo;
        //    if (propertyInfo == null)
        //    {
        //        return null;
        //    }
        //    var o = propertyInfo.GetValue(model, null);
        //    return o;
        //}

        public static T Get<T>(this ISimpleRepository simpleRepos, object id, string idName = "Id") where T : class
        {
            //Expression<Func<TSource, TResult>> selector
            var predicate = CreatePredicate<T>(id, idName);
            var theOne = simpleRepos.Get(predicate);
            return theOne;
        }

        public static Expression<Func<T, bool>> CreatePredicate<T>(object propValue, string propName)
        {
            var paramExpression = Expression.Parameter(typeof(T));
            //x => x.{propName}
            var propExpression = CreatePropertyExpression(paramExpression, propName);
            
            var propValueExpression = Expression.Constant(propValue);
            var express = Expression.Equal(propExpression, propValueExpression);
            //var andExp = Expression.AndAlso(e1, e2);
            var lambda = Expression.Lambda<Func<T, bool>>(express, paramExpression);
            return lambda;
        }
        
        private static MemberExpression CreatePropertyExpression(ParameterExpression paramExpression, string propName)
        {
            //x => x.{propName}
            var memberExpression = Expression.Property(paramExpression, propName);
            return memberExpression;
        }
    }

    #region memory impl

    public class SimpleMemoryRepository : ISimpleRepository
    {
        public BaseSubTypeRelationMap Relations { get; set; }
        public IDictionary<Type, IList<object>> DicValues { get; set; }
        public SimpleMemoryRepository()
        {
            Relations = BaseSubTypeRelationMap.Instance;
            DicValues = new ConcurrentDictionary<Type, IList<object>>();
        }

        public IEnumerable<T> Query<T>() where T : class
        {
            return Items<T>().AsQueryable();
        }

        public T Get<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var theOne = Query<T>().SingleOrDefault(predicate.Compile());
            return theOne;
        }
        
        public void Add<T>(params T[] entities) where T : class
        {
            var GetPropValue = GetIdValueFunc<T>();
            var models = GetSaveModels<T>();

            foreach (var entity in entities)
            {
                var theId = GetPropValue(entity);
                var theOne = models.SingleOrDefault(x => theId.Equals(GetPropValue(x)));
                if (theOne != null)
                {
                    throw new InvalidOperationException("数据已经存在：" + theId);
                }
                models.Add(entity);
            }
        }
        
        public void Update<T>(params T[] entities) where T : class
        {
            var GetPropValue = GetIdValueFunc<T>();
            var models = GetSaveModels<T>();
            foreach (var entity in entities)
            {
                if (!models.Contains(entity))
                {
                    var theId = GetPropValue(entity);
                    var theOne = models.SingleOrDefault(x => theId.Equals(GetPropValue(x)));
                    if (theOne == null)
                    {
                        throw new InvalidOperationException("未找到项" + entity);
                    }
                    var indexOf = models.IndexOf(theOne);
                    models.Remove(theOne);
                    models.Insert(indexOf, entity);
                }
            }
        }

        public void Delete<T>(params T[] entities) where T : class
        {
            var GetPropValue = GetIdValueFunc<T>();
            var models = GetSaveModels<T>();
            foreach (var entity in entities)
            {
                if (models.Contains(entity))
                {
                    models.Remove(entity);
                }
                else
                {
                    var theId = GetPropValue(entity);
                    var theOne = models.SingleOrDefault(x => theId.Equals(GetPropValue(x)));
                    if (theOne != null)
                    {
                        models.Remove(theOne);
                    }
                }
            }
        }
        
        public void Truncate<T>() where T : class
        {
            var models = GetSaveModels<T>();
            var keepModels = models.Where(x => !(x is T)).ToList();
            var baseType = GetBaseType<T>();
            DicValues[baseType] = keepModels;
        }
        
        protected IList<T> Items<T>()
        {
            var type = typeof(T);

            var baseType = Relations.GetBaseType(type);
            if (baseType == null)
            {
                baseType = type;
            }

            if (!DicValues.ContainsKey(baseType))
            {
                DicValues[baseType] = new List<object>();
            }
            return DicValues[baseType].Where(x => x is T).Cast<T>().ToList();
        }

        protected IList<object> GetSaveModels<T>()
        {
            var type = typeof(T);

            var baseType = Relations.GetBaseType(type);
            if (baseType == null)
            {
                baseType = type;
            }

            if (!DicValues.ContainsKey(baseType))
            {
                DicValues[baseType] = new List<object>();
            }
            return DicValues[baseType];
        }

        protected Type GetBaseType<T>()
        {
            var type = typeof(T);
            var baseType = Relations.GetBaseType(type);
            if (baseType == null)
            {
                baseType = type;
            }
            return baseType;
        }

        public SimpleMemoryRepository Init<T>(IList<T> items)
        {
            var models = GetSaveModels<T>();
            foreach (var item in items)
            {
                models.Add(item);
            }
            return this;
        }

        public Func<object, object> GetIdValueFunc<T>()
        {
            //todo cache and validate
            Func<object, object> selector = arg => {return GetThePropValue(arg, "Id"); };
            return selector;
        }

        private static object GetThePropValue(object model, string propName = "Id")
        {
            //x => x.{propName}
            var paramExpression = Expression.Parameter(model.GetType());
            var memberExpression = CreatePropertyExpression(paramExpression, propName);
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                return null;
            }
            var o = propertyInfo.GetValue(model, null);
            return o;
        }

        private static MemberExpression CreatePropertyExpression(ParameterExpression paramExpression, string propName)
        {
            //x => x.{propName}
            var memberExpression = Expression.Property(paramExpression, propName);
            return memberExpression;
        }
    }

    public class BaseSubTypeRelationMap
    {
        //for auto fix multi type
        public IDictionary<Type, Type> Mappings { get; set; }

        public BaseSubTypeRelationMap()
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

        public Type GetBaseType(Type subType)
        {
            return Mappings.ContainsKey(subType) ? Mappings[subType] : null;
        }

        public static BaseSubTypeRelationMap Instance = new BaseSubTypeRelationMap();
    }

    #endregion
}