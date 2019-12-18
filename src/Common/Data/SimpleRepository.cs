using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

// 20191218: V 0.1.0, first release
// ReSharper disable CheckNamespace
namespace Common.Data
{
    public interface ISimpleEntity
    {
        object Id { get; set; }
    }

    public interface ISimpleEntity<TPk> : ISimpleEntity
    {
        new TPk Id { get; set; }
    }

    public abstract class BaseSimpleEntity<TPk> : ISimpleEntity<TPk>
    {
        object ISimpleEntity.Id
        {
            get => Id;
            set => Id = (TPk)value;
        }

        public virtual TPk Id { get; set; }

        private readonly TPk _defaultIdValue = default(TPk);
        private int? _oldHashCode;
        public override int GetHashCode()
        {
            // once we have a hashcode we'll never change it
            if (_oldHashCode.HasValue)
                return _oldHashCode.Value;
            // when this instance is new we use the base hash code
            // and remember it, so an instance can NEVER change its
            // hash code.
            var thisIsNew = Equals(Id, _defaultIdValue);
            if (thisIsNew)
            {
                _oldHashCode = base.GetHashCode();
                return _oldHashCode.Value;
            }
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as BaseSimpleEntity<TPk>;
            if (other == null) return false;

            var thisIsNew = Equals(Id, _defaultIdValue);
            var otherIsNew = Equals(other.Id, _defaultIdValue);

            if (thisIsNew && otherIsNew)
                return ReferenceEquals(this, other);

            return Id.Equals(other.Id);
        }

        public static bool operator ==(BaseSimpleEntity<TPk> lhs, BaseSimpleEntity<TPk> rhs)
        {
            return Equals(lhs, rhs);
        }
        public static bool operator !=(BaseSimpleEntity<TPk> lhs, BaseSimpleEntity<TPk> rhs)
        {
            return !Equals(lhs, rhs);
        }
    }

    public interface ISimpleRepository
    {
        IQueryable<T> Query<T>() where T : ISimpleEntity;
        T Get<T>(object id) where T : ISimpleEntity;


        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        void Add<T>(params T[] entities) where T : ISimpleEntity<T>;

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        void Add<T>(T entity, object id = null) where T : ISimpleEntity;

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        void Update<T>(params T[] entities) where T : ISimpleEntity;

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        void SaveOrUpdate<T>(params T[] entities) where T : ISimpleEntity;

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        void Delete<T>(params T[] entities) where T : ISimpleEntity;

        /// <summary>
        /// 删除全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Truncate<T>() where T : ISimpleEntity;

        /// <summary>
        /// 提交数据
        /// </summary>
        void Flush();

        /// <summary>
        /// 用于某些追求性能的查询场景
        /// </summary>
        void QueryOnly(bool queryOnly);
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

        public IQueryable<T> Query<T>() where T : ISimpleEntity
        {
            return Items<T>().AsQueryable();
        }

        public T Get<T>(object id) where T : ISimpleEntity
        {
            return Query<T>().SingleOrDefault(x => x.Id.Equals(id));
        }

        public void Add<T>(params T[] entities) where T : ISimpleEntity<T>
        {
            foreach (var entity in entities)
            {
                var models = GetSaveModels<T>();
                models.Add(entity);
            }
        }

        public void Add<T>(T entity, object id = null) where T : ISimpleEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (id != null)
            {
                entity.Id = id;
            }

            var models = GetSaveModels<T>();
            var theOne = Get<T>(entity.Id);
            if (theOne != null)
            {
                throw new InvalidOperationException("数据已经存在：" + entity.Id);
            }
            models.Add(entity);
        }

        public void Update<T>(params T[] entities) where T : ISimpleEntity
        {
            var models = GetSaveModels<T>();
            foreach (var entity in entities)
            {
                var theOne = Get<T>(entity.Id);
                if (theOne == null)
                {
                    throw new InvalidOperationException("未找到项" + entity.Id);
                }

                if (ReferenceEquals(entity, theOne))
                {
                    return;
                }

                var indexOf = models.IndexOf(theOne);
                models.Remove(theOne);
                models.Insert(indexOf, entity);
            }
        }

        public void SaveOrUpdate<T>(params T[] entities) where T : ISimpleEntity
        {
            var models = GetSaveModels<T>();
            foreach (var entity in entities)
            {
                var theOne = Get<T>(entity.Id);
                if (theOne != null)
                {
                    if (ReferenceEquals(entity, theOne))
                    {
                        return;
                    }
                    var indexOf = models.IndexOf(theOne);
                    models.Remove(theOne);
                    models.Insert(indexOf, entity);
                }
                else
                {
                    models.Add(entity);
                }
            }
        }

        public void Delete<T>(params T[] entities) where T : ISimpleEntity
        {
            var models = GetSaveModels<T>();
            foreach (var entity in entities)
            {
                var theOne = Get<T>(entity.Id);
                if (theOne != null)
                {
                    models.Remove(theOne);
                }
            }
        }

        public void Truncate<T>() where T : ISimpleEntity
        {
            var models = GetSaveModels<T>();
            var keepModels = models.Where(x => !(x is T)).ToList();
            var baseType = GetBaseType<T>();
            DicValues[baseType] = keepModels;
        }

        public void Flush()
        {
        }

        public void QueryOnly(bool queryOnly)
        {
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