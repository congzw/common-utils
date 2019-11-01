using Common.Logs.Api;
using Common.Logs.Api.Proxy;
using Common.Logs.Refs;
using Common.Logs.Refs.ApiProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DemoLogApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            SimpleLogSettings.DefaultPrefix = "DemoLogApi";

            #region only for test proxy client call!

            var apiClientConfig = new ApiClientConfig()
            {
                FailTimeoutMilliseconds = 200,
                BaseUri = "http://localhost:10005/api/log"
            };
            var webApiHelper = WebApiHelper.Resolve();
            webApiHelper.LogMessage = true;
            var simpleApiClient = new SimpleApiClient(webApiHelper, apiClientConfig);

            var webApiProxySmartWrapper = SimpleApiClientSmartWrapper.Resolve();
            webApiProxySmartWrapper.TestConnectionGetApiUri = "http://localhost:10005/api/log/getDate";
            webApiProxySmartWrapper.TestTimeoutMilliseconds = apiClientConfig.FailTimeoutMilliseconds;
            webApiProxySmartWrapper.Reset(simpleApiClient);

            services.AddSingleton<ISimpleApiClient>(webApiProxySmartWrapper);

            #endregion

            services.AddSingleton<ILogApi, LogApi>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
