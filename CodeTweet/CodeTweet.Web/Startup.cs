﻿using CodeTweet.IdentityDal;
using CodeTweet.IdentityDal.Model;
using CodeTweet.ImagesDal;
using CodeTweet.Queueing;
using CodeTweet.Queueing.ZeroMQ;
using CodeTweet.TweetsDal;
using CodeTweet.Web.Managers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeTweet.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationIdentityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Identity")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            var documentDbConfiguration = new DocumentDbConfiguration();
            Configuration.GetSection("DocumentDB").Bind(documentDbConfiguration);
            services.AddSingleton(documentDbConfiguration);

            var imagesDbConfiguration = new ImagesDbConfiguration();
            Configuration.GetSection("ImagesDB").Bind(imagesDbConfiguration);
            services.AddSingleton(imagesDbConfiguration);

            services.AddTransient<IImagesRepository, ImagesRepository>();

            services.AddTransient<ITweetsRepository, TweetsRepository>();

            ZeroConfiguration zeroConfiguration = new ZeroConfiguration();
            Configuration.GetSection("ZeroMq").Bind(zeroConfiguration);
            services.AddSingleton<INotificationEnqueue>(provider => new ZeroNotificationEnqueue(zeroConfiguration));

            services.AddTransient<TweetsManager>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
