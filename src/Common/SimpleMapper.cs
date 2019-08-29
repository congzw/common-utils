using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

// ReSharper disable CheckNamespace
namespace Common
{
    public interface ISimpleMapper
    {
        ExpandoObject ShapeData(object source, IEnumerable<string> fields, bool fillNullIfNotExist);
    }

    public class SimpleMapper : ISimpleMapper
    {
        public SimpleMapper()
        {
            PropertyInfos = new Dictionary<Type, IList<PropertyInfo>>();
        }
        
        public IDictionary<Type, IList<PropertyInfo>> PropertyInfos { get; set; }
        
        public ExpandoObject ShapeData(object source, IEnumerable<string> fields, bool fillNullIfNotExist)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            var fieldList = fields.ToList();

            var propertyInfos = GetOrCreate(source.GetType());
            var shapedObject = new ExpandoObject() as IDictionary<string, object>;

            foreach (var field in fieldList)
            {
                var propertyInfo = propertyInfos.SingleOrDefault(x =>
                    field.Equals(x.Name, StringComparison.OrdinalIgnoreCase));

                if (propertyInfo != null)
                {
                    var pValue = propertyInfo.GetValue(source);
                    shapedObject.Add(field, pValue);
                }
                else
                {
                    if (fillNullIfNotExist)
                    {
                        shapedObject.Add(field, null);
                    }
                }
            }
            return (ExpandoObject) shapedObject;
        }

        private IList<PropertyInfo> GetOrCreate(Type type)
        {
            if (PropertyInfos.ContainsKey(type))
            {
                return PropertyInfos[type];
            }

            var propertyInfos = type.GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).ToList();
            PropertyInfos[type] = propertyInfos;
            return propertyInfos;
        }

        #region for di extensions

        private static readonly Lazy<ISimpleMapper> LazyInstance = new Lazy<ISimpleMapper>(() => new SimpleMapper());
        public static Func<ISimpleMapper> Resolve { get; set; } = () => LazyInstance.Value;

        #endregion
    }

    public static class SimpleMapperExtensions
    {
        public static ExpandoObject ShapeData(this ISimpleMapper simpleMapper, object source, string fields, bool fillNullIfNotExist = false)
        {
            var items = fields.Split(',');
            var fixItems = items.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim());
            return simpleMapper.ShapeData(source, fixItems, fillNullIfNotExist);
        }

        public static ExpandoObject AsShape(this object source, string fields, bool fillNullIfNotExist = false)
        {
            var simpleMapper = SimpleMapper.Resolve();
            return ShapeData(simpleMapper, source, fields, fillNullIfNotExist);
        }
    }

    #region todo replace refletions with expressions?

    //borrow from https://github.com/dotarj/DynamicMapper
    //public static class Mapper<T>
    //{
    //    private static readonly MethodInfo DictionaryAddMethod = typeof(IDictionary<string, object>).GetMethod("Add", new[] { typeof(string), typeof(object) });

    //    private static readonly MethodInfo CollectionContainsMethod = typeof(ICollection<PropertyInfo>).GetMethod("Contains", new[] { typeof(PropertyInfo) });

    //    private static readonly Action<T, ExpandoObject, PropertyInfo[]> MapImpl;

    //    static Mapper()
    //    {
    //        var sourceExpression = Expression.Parameter(typeof(T), "source");
    //        var targetExpression = Expression.Parameter(typeof(ExpandoObject), "target");
    //        var propertiesExpression = Expression.Parameter(typeof(PropertyInfo[]), "properties");

    //        var expressions = typeof(T)
    //            .GetProperties()
    //            .Where(property => property.CanRead && !property.GetIndexParameters().Any())
    //            .Select(property =>
    //                // if (properties.Contains({property}))
    //                Expression.IfThen(Expression.Call(propertiesExpression, CollectionContainsMethod, Expression.Constant(property, typeof(PropertyInfo))),
    //                    // target.Add({property.Name}, (object)source.{property.Name});
    //                    Expression.Call(targetExpression, DictionaryAddMethod,
    //                        Expression.Constant(property.Name, typeof(string)),
    //                        Expression.Convert(Expression.Property(sourceExpression, property), typeof(object)))));

    //        Mapper<T>.MapImpl = Expression.Lambda<Action<T, ExpandoObject, PropertyInfo[]>>(Expression.Block(expressions), sourceExpression, targetExpression, propertiesExpression).Compile();
    //    }

    //    /// <summary>
    //    /// Maps the <paramref name="properties"/> from <paramref name="source"/> to a dynamic object.
    //    /// </summary>
    //    /// <param name="source">The source object to map from.</param>
    //    /// <param name="properties">A <see cref="PropertyInfo[]"/> with properties to map.</param>
    //    /// <returns>The dynamic object containing the mapped properties.</returns>
    //    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    //    /// <exception cref="ArgumentNullException"><paramref name="properties"/> is null.</exception>
    //    public static dynamic Map(T source, params PropertyInfo[] properties)
    //    {
    //        if (source == null)
    //        {
    //            throw new ArgumentNullException("source");
    //        }

    //        if (properties == null)
    //        {
    //            throw new ArgumentNullException("properties");
    //        }

    //        dynamic target = new ExpandoObject();

    //        MapImpl(source, target, properties);

    //        return target;
    //    }

    //    /// <summary>
    //    /// Maps the <paramref name="properties"/> from <paramref name="source"/> to the target object.
    //    /// </summary>
    //    /// <param name="source">The source object to map from.</param>
    //    /// <param name="target">The target object to map to.</param>
    //    /// <param name="properties">A <see cref="PropertyInfo[]"/> with properties to map.</param>
    //    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    //    /// <exception cref="ArgumentNullException"><paramref name="properties"/> is null.</exception>
    //    public static void Map(T source, ExpandoObject target, params PropertyInfo[] properties)
    //    {
    //        if (source == null)
    //        {
    //            throw new ArgumentNullException("source");
    //        }

    //        if (target == null)
    //        {
    //            throw new ArgumentNullException("target");
    //        }

    //        if (properties == null)
    //        {
    //            throw new ArgumentNullException("properties");
    //        }

    //        MapImpl(source, target, properties);
    //    }
    //}


    #endregion
}
