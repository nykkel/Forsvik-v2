using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Forsvik.Core.Database;
using System;
using System.IO;
using System.Linq;
using System.Net;
using Forsvik.Core.Database.Repositories;
using Forsvik.Core.Model.Interfaces;
using forsvikapi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

namespace forsvikapi
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSpaStaticFiles(configuration: options => { options.RootPath = "app"; });
            services.AddControllers();
            services.AddCors(options =>
            {
                var corsSource = Configuration.GetValue<string>("WithOrigin");
                options.AddPolicy("VueCorsPolicy", builder =>
                {
                    builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(corsSource);
                });
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = null;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = null;
            });

            RegisterAuthentication(services);

            services.AddMvc(option => 
            {
                option.EnableEndpointRouting = false;
                option.Filters.Add<ErrorHandlingFilter>();
            });

            RegisterServices(services);

            services.AddDbContext<ArchivingContext>((builder =>
            {
                var connectionString = Configuration["ConnectionStrings:ForsvikDb"];
                builder.UseSqlServer(connectionString);
            }));
        }

        private void RegisterAuthentication(IServiceCollection services)
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => 
                {
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = false;
                });
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<ArchivingContext>();
            services.AddScoped<ArchivingRepository>();
            services.AddScoped<ResourceDocumentStore>();
            //services.AddScoped<IFileRepository, DocumentFileRepository>();
            services.AddScoped<AdminRepository>();            
            services.AddScoped<LogRepository>();
            services.AddScoped<ErrorHandlingFilter>();            
            services.AddScoped<SearchRepository>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<UserService>();            
            services.AddScoped<IFileRepository, DiskFileRepository>();
            services.AddSingleton(Configuration);   
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                //app.UseDeveloperExceptionPage();
                HandleExceptions(app);
            //}            

            app.UseCors("VueCorsPolicy");

            app.UseAuthentication();            

            app.UseMvc();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });            
            app.UseSpaStaticFiles();
            app.UseSpa(configuration: builder =>
            {
                if (env.IsDevelopment())
                {
                    builder.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                }
            });
        }

        public void HandleExceptions(IApplicationBuilder app)
        { 
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();

                    if (exceptionHandlerPathFeature != null)
                    {
                        var log = app.ApplicationServices.GetService<ILogService>();
                        log?.Error("App exception1: " + exceptionHandlerPathFeature.Error.Message);

                        await context.Response.WriteAsync(
                            exceptionHandlerPathFeature.Error.Message);
                    }
                });
            });
            app.UseHsts();
        }
    }
}