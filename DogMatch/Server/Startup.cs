using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DogMatch.Server.Data;
using DogMatch.Server.Data.Models;
using DogMatch.Server.Services;
using AutoMapper;
using DogMatch.Server.Infrastructure;
using Microsoft.Extensions.FileProviders;
using System.IO;
using DogMatch.Server.Data.Repositories;

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

            services.AddDefaultIdentity<DogMatchUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<DogMatchDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<DogMatchUser, DogMatchDbContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddControllersWithViews();
            services.AddRazorPages();

            // scoped services
            services.AddScoped<IDogService, DogService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ITemperamentService, TemperamentService>();

            // scoped repositories
            services.AddScoped<IDogRepository, DogRepository>();            
            services.AddScoped<ITemperamentRepository, TemperamentRepository>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfileServerConfiguration());
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

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images/ProfileImages")),
                RequestPath = "/ProfileImage"
            });
        }
    }
}
