using System;

namespace Common
{
    public interface IServiceProviderLocator
    {
        IServiceProvider GetProvider();
    }

    public sealed class ServiceProviderLocator : IServiceProviderLocator
    {
        public IServiceProvider GetProvider()
        {
            return Resolve == null ? NullServiceProvider.Instance : Resolve();
        }

        public static IServiceProviderLocator Instance = new ServiceProviderLocator();

        #region for easy use and for extensions

        //for easy use
        public static IServiceProvider Provider => Instance.GetProvider();

        // for extensions
        public static Func<IServiceProvider> Resolve = null;

        class NullServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return null;
            }
            public static IServiceProvider Instance = new NullServiceProvider();
        }

        #endregion
    }


    #region extensions for aspnet core
        
    //using Microsoft.AspNetCore.Builder;
    //using Microsoft.AspNetCore.Http;
    //using Microsoft.AspNetCore.Mvc.Infrastructure;
    //using Microsoft.Extensions.DependencyInjection;
    //using Microsoft.Extensions.DependencyInjection.Extensions;

    ////how to use:
    ////1 setup => services.AddServiceProviderLocatorHttp();
    ////2 use => app.UseServiceProviderLocatorHttp();
    //public class ServiceProviderLocatorHttp : IServiceProviderLocator
    //{
    //    private readonly IServiceProvider _rootServiceProvider;

    //    public ServiceProviderLocatorHttp(IServiceProvider rootServiceProvider)
    //    {
    //        _rootServiceProvider = rootServiceProvider;
    //    }

    //    public IServiceProvider GetProvider()
    //    {
    //        var httpContext = GetHttpContext();
    //        if (httpContext == null)
    //        {
    //            return _rootServiceProvider;
    //        }
    //        return httpContext.RequestServices;
    //    }

    //    private HttpContext GetHttpContext()
    //    {
    //        var contextAccessor = _rootServiceProvider.GetService<IHttpContextAccessor>();
    //        if (contextAccessor == null)
    //        {
    //            throw new InvalidOperationException("没有初始化: IHttpContextAccessor");
    //        }
    //        return contextAccessor.HttpContext;
    //    }
    //}

    //public static class ServiceProviderLocatorHttpExtensions
    //{
    //    public static IServiceCollection AddServiceProviderLocatorHttp(this IServiceCollection services)
    //    {
    //        services.AddHttpContextAccessor();
    //        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
    //        services.AddSingleton<IServiceProviderLocator, ServiceProviderLocatorHttp>();
    //        return services;
    //    }

    //    public static IApplicationBuilder UseServiceProviderLocatorHttp(this IApplicationBuilder app)
    //    {
    //        ServiceProviderLocator.Resolve = () => app.ApplicationServices.GetRequiredService<IServiceProviderLocator>().GetProvider();
    //        return app;
    //    }

    //    public static void RunInScope(this IServiceProvider sp, Action<IServiceProvider> action)
    //    {
    //        using (var scope = sp.CreateScope())
    //        {
    //            action(scope.ServiceProvider);
    //        }
    //    }
    //}

    #endregion
}
