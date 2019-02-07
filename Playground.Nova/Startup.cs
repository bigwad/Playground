using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Starcounter.Nova;
using Starcounter.Nova.Hosting;
using Starcounter.Nova.Options;
using Starcounter.Nova.Extensions.DependencyInjection;
using Starcounter.Nova.Bluestar;
using Starcounter.Nova.Hosting.BindingExtensions;
using Starcounter.Nova.Abstractions;
using Playground.Nova.Models;

namespace Playground.Nova
{
    public class Startup
    {
        public const string StarcounterDatabaseName = "PlaygroundNova";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            string cn = "data source=192.168.1.73;initial catalog=Playground;persist security info=True;user id=sa;password=*****;Connection Timeout=30;";
            services.AddDbContext<PlaygroundContext>(options => options.UseSqlServer(cn));

            this.ConfigureStarcounterDatabase(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void ConfigureStarcounterDatabase(IServiceCollection services)
        {
            string home = GetHomePath();
            string path = Path.Combine(home, "Documents/Starcounter.Nova/Databases", StarcounterDatabaseName);

            // if (!Directory.Exists(path))
            // {
            //     Directory.CreateDirectory(path);
            // }

            if (!StarcounterOptions.TryOpenExisting(path))
            {
                Directory.CreateDirectory(path);
                ScCreateDb.Execute(path);
            }

            IAppHost host = new AppHostBuilder()
                .UseDatabase(path)
                .Build();

            services.AddStarcounter(host);
        }

        /// <summary>
        /// Returns path to the user "Home" folder.
        /// Throws <see cref="DirectoryNotFoundException"/> if the user home environment variable is not set.
        /// https://en.wikipedia.org/wiki/Home_directory
        /// </summary>
        /// <returns></returns>
        public string GetHomePath()
        {
            string path = null;
            string[] names = new string[] { "USERPROFILE", "HOME" };

            foreach (string name in names)
            {
                path = Environment.GetEnvironmentVariable(name);

                if (!string.IsNullOrEmpty(path))
                {
                    break;
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new DirectoryNotFoundException("Unable to determine user home folder from the environment variables.");
            }

            return path;
        }
    }
}
