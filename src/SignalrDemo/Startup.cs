using System.Collections.Generic;
using Common.SignalR.ClientMonitors;
using Common.SignalR.Scoped;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SignalrDemo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddClientMonitors();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var fileServerOptions = new FileServerOptions();
            var defaultPages = new List<string>();
            defaultPages.Add("index.html");
            fileServerOptions.DefaultFilesOptions.DefaultFileNames = defaultPages;

            app.UseFileServer(fileServerOptions);

            app.UseSignalR(routes =>
            {
                routes.MapHub<_AnyHub>("/DemoHub");
            });
        }
    }
}
