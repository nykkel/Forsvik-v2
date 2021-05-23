using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Forsvik.Core.Database;
using System;
using Forsvik.Core.Database.Repositories;
using Forsvik.Core.Model.Interfaces;
using forsvikapi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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
                options.AddPolicy("VueCorsPolicy", builder =>
                {
                    builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("http://forsvikarkiv.azurewebsites.net");
                    //.WithOrigins("http://localhost:5001");
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
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.Authority = Configuration["Okta:Authority"];
            //        options.Audience = "api://default";
            //    });
            services.AddMvc(option => 
            {
                option.EnableEndpointRouting = false;
                option.Filters.Add<ErrorHandlingFilter>();
            });

            RegisterServices(services);
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
            services.AddScoped<DocumentRepository>();
            services.AddScoped<AdminRepository>();            
            services.AddScoped<LogRepository>();
            services.AddScoped<ErrorHandlingFilter>();            
            services.AddScoped<SearchRepository>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<UserService>();            
            services.AddScoped<IFileRepository, BlobFileRepository>();
            services.AddSingleton(Configuration);            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
            //}            

            app.UseCors("VueCorsPolicy");

            //dbContext.Database.EnsureCreated();
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
    }
}