using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public interface IServiceLocator
    {
        T GetService<T>();
        IEnumerable<T> GetServices<T>();
        object GetService(Type type);
        IEnumerable<object> GetServices(Type type);
    }

    public class ServiceLocator
    {
        private static IServiceLocator diProxy = new NullServiceLocator();

        public static IServiceLocator Current => diProxy ?? throw new Exception("You should Initialize the ServiceLocator before using it.");

        public static void Initialize(IServiceLocator proxy)
        {
            diProxy = proxy;
        }
    }

    public class NullServiceLocator : IServiceLocator
    {
        public T GetService<T>()
        {
            return default(T);
        }

        public IEnumerable<T> GetServices<T>()
        {
            return Enumerable.Empty<T>();
        }

        public object GetService(Type type)
        {
            return null;
        }

        public IEnumerable<object> GetServices(Type type)
        {
            return Enumerable.Empty<object>();
        }
    }

    public interface ISimpleServiceResolver
    {
        void Add<BaseT, SubT>(Func<SubT> factory) where SubT : BaseT;
        void Remove<T>();
    }

    public static class SimpleServiceResolverExtensions
    {
        public static void Add<T>(this ISimpleServiceResolver resolver, Func<T> factory)
        {
            resolver.Add<T, T>(factory);
        }
    }

    public interface ISimpleServiceLocator : ISimpleServiceResolver, IServiceLocator
    {
    }

    public class SimpleServiceLocator : ISimpleServiceLocator
    {
        public RealTypeFactories Factories { get; set; }
        public TypeRegisterAsRefs TypeRefs { get; set; }

        public SimpleServiceLocator()
        {
            Factories = new RealTypeFactories();
            TypeRefs = new TypeRegisterAsRefs();
        }

        public void Add<BaseT, SubT>(Func<SubT> factory) where SubT : BaseT
        {
            Func<object> f = () => factory();

            var baseType = typeof(BaseT);
            var realType = typeof(SubT);

            TypeRefs.AddIfNotExist(baseType, realType);
            TypeRefs.AddIfNotExist(realType, realType);

            var funcRef = Factories.GetFuncRef(realType);
            if (funcRef == null)
            {
                Factories.AddFuncRef(realType, f);
            }
        }

        public void Remove<T>()
        {
            var realType = typeof(T);
            TypeRefs.Remove(realType);
            Factories.RemoveFuncRef(realType);
        }

        public T GetService<T>()
        {
            var asType = typeof(T);
            var service = GetService(asType);
            return (T)service;
        }

        public IEnumerable<T> GetServices<T>()
        {
            var asType = typeof(T);
            var services = GetServices(asType);
            return services.Cast<T>().ToList();
        }

        public object GetService(Type asType)
        {
            var services = GetServices(asType).ToList();
            if (services.Count < 1)
            {
                return null;
            }
            if (services.Count > 1)
            {
                throw new Exception("find more than one service for type: " + asType.Name);
            }
            var service = services[0];
            return service;
        }

        public IEnumerable<object> GetServices(Type asType)
        {
            var realTypes = TypeRefs.GetRealTypes(asType);
            if (realTypes.Length == 0)
            {
                return Enumerable.Empty<object>();
            }

            var funcRefs = Factories.GetFuncRefs(realTypes);
            var objects = funcRefs.Select(x => x.Invoke()).ToList();
            return objects;

        }

        public class TypeRegisterAsRef
        {
            public TypeRegisterAsRef(Type asType, Type realType)
            {
                AsType = asType;
                RealType = realType;
            }

            public Type AsType { get; set; }
            public Type RealType { get; set; }
        }

        public class TypeRegisterAsRefs
        {
            public TypeRegisterAsRefs()
            {
                Refs = new List<TypeRegisterAsRef>();
            }

            public List<TypeRegisterAsRef> Refs { get; set; }

            public bool HasRegistered(Type asType)
            {
                return Refs.Exists(x => x.AsType == asType);
            }

            public Type[] GetRealTypes(Type asType)
            {
                var realTypes = Refs.Where(x => x.AsType == asType).Select(x => x.RealType).ToArray();
                return realTypes;
            }

            public void AddIfNotExist(Type asType, Type realType)
            {
                var theOne = Refs.SingleOrDefault(x => x.RealType == realType && x.AsType == asType);
                if (theOne != null)
                {
                    return;
                }
                Refs.Add(new TypeRegisterAsRef(asType, realType));
            }

            public void Remove(Type realType)
            {
                Refs.RemoveAll(x => x.RealType == realType);
            }
        }

        public class RealTypeFactories
        {
            public IDictionary<Type, Func<object>> FuncRefs { get; set; }

            public RealTypeFactories()
            {
                FuncRefs = new ConcurrentDictionary<Type, Func<object>>();
            }

            public IList<Func<object>> GetFuncRefs(params Type[] realTypes)
            {
                var funcRefs = new List<Func<object>>();
                foreach (var realType in realTypes)
                {
                    if (FuncRefs.ContainsKey(realType))
                    {
                        funcRefs.Add(FuncRefs[realType]);
                    }
                }
                return funcRefs;
            }

            public Func<object> GetFuncRef(Type realType)
            {
                return FuncRefs.ContainsKey(realType) ? FuncRefs[realType] : null;
            }

            public void AddFuncRef(Type realType, Func<object> funcRef)
            {
                FuncRefs.Add(realType, funcRef);
            }
            public void RemoveFuncRef(Type realType)
            {
                if (FuncRefs.ContainsKey(realType))
                {
                    FuncRefs.Remove(realType);
                }
            }
        }
    }

    #region extensions demo for Microsoft.Extensions.DependencyInjection

    ////how to use:
    ////1 setup => services.AddSingleton<IServiceLocator, HttpRequestServiceLocator>();
    ////2 use => ServiceLocator.Initialize(app.ApplicationServices.GetService<IServiceLocator>());
    //public class HttpRequestServiceLocator : IServiceLocator
    //{
    //    private readonly IServiceProvider _serviceProvider;

    //    public HttpRequestServiceLocator(IServiceProvider serviceProvider)
    //    {
    //        _serviceProvider = serviceProvider;
    //    }

    //    public T GetService<T>()
    //    {
    //        var contextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
    //        var httpContext = contextAccessor.HttpContext;
    //        if (httpContext == null)
    //        {
    //            return _serviceProvider.GetService<T>();
    //        }
    //        return contextAccessor.HttpContext.RequestServices.GetService<T>();
    //    }

    //    public IEnumerable<T> GetServices<T>()
    //    {
    //        var contextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
    //        var httpContext = contextAccessor.HttpContext;
    //        if (httpContext == null)
    //        {
    //            return _serviceProvider.GetServices<T>();
    //        }
    //        return contextAccessor.HttpContext.RequestServices.GetServices<T>();
    //    }

    //    public object GetService(Type type)
    //    {
    //        var contextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
    //        var httpContext = contextAccessor.HttpContext;
    //        if (httpContext == null)
    //        {
    //            return _serviceProvider.GetService(type);
    //        }
    //        return contextAccessor.HttpContext.RequestServices.GetService(type);
    //    }

    //    public IEnumerable<object> GetServices(Type type)
    //    {
    //        var contextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
    //        var httpContext = contextAccessor.HttpContext;
    //        if (httpContext == null)
    //        {
    //            return _serviceProvider.GetServices(type);
    //        }
    //        return contextAccessor.HttpContext.RequestServices.GetServices(type);
    //    }
    //}

    #endregion
}
