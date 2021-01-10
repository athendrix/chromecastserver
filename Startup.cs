using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChromecastServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();//.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("This is a Chromecast server.\r\n" +
                    "Links should take the form http://<This Server>/cast?receiver=<Chromecast Device IP>&mi=<Path To Media>&random=true&repeat=true\r\n" +
                    "You can add many mi entries.\r\n" +
                    "It is recommended to give your Chromecast devices static IP reservations.\r\n" +
                    "If random is set to true, the media files passed in will be loaded in a random order, and the Chromecast should shuffle on repeat\r\n" +
                    "If repeat is set to true, the media will repeat the playlist on completion. To repeat one, only load one file.\r\n" +
                    "Future versions will support:\r\n" +
                    "\tWeb folder paths in addition to just files\r\n" + 
                    "\tPOST requests in addition to GET requests\r\n" +
                    "Hostnames do not work for Chromecast devices because DNS for containers is often different.\r\n" +
                    "Hostnames should work for file paths, since the Chromecast itself is doing the request.\r\n" + 
                    "Hence why static IP reservations is recommended for the Chromecasts.");
                });
                endpoints.MapControllerRoute("Cast", "{controller}/{action=Get}");
            });
        }
    }
}
