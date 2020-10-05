using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DogMatch.Domain.Data;
using DogMatch.Domain.Data.Models;
using DogMatch.Domain.Services;
using AutoMapper;
using DogMatch.Domain.Infrastructure;
using Microsoft.Extensions.FileProviders;
using DogMatch.Domain.Data.Repositories;
using System;

namespace DogMatch.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(Options => Options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddDbContext<DogMatchDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<DogMatchUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;                
            })          
            .AddEntityFrameworkStores<DogMatchDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<DogMatchUser, DogMatchDbContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(12);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "DogMatch_Session";                
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

            // scoped services
            services.AddScoped<IDogService, DogService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ITemperamentService, TemperamentService>();
            services.AddScoped<IBiographyService, BiographyService>();
            services.AddScoped<IUserService, UserService>();

            // scoped repositories
            services.AddScoped<IDogRepository, DogRepository>();            
            services.AddScoped<ITemperamentRepository, TemperamentRepository>();
            services.AddScoped<IBiographyRepository, BiographyRepository>();
            services.AddScoped<IColorRepository, ColorRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDogImagesRepository, DogImagesRepository>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfileDomainConfiguration());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseSession(); // must call after UseRouting, before UseEndpoints           

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
            
            // use static files for profile images
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Configuration.GetValue<string>("FilePaths:ProfileImageDir")),
                RequestPath = "/ProfileImage"
            });

            // use static files for album images
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Configuration.GetValue<string>("FilePaths:AlbumImageDir")),
                RequestPath = "/AlbumImage"
            });
        }
    }
}
