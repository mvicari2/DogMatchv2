using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Tewr.Blazor.FileReader;
using DogMatch.Client.Services;
using Radzen;

namespace DogMatch.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("DogMatch.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("DogMatch.ServerAPI"));

            builder.Services.AddApiAuthorization();
            builder.Services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);

            /// Initially configured all client state management services w/Singleton lifetime,
            /// but due to changing HttpClient to scoped for .net 5 upgrade 
            /// (because of transient memory leaks), these services now must also have scoped lifetimes.
            /// However, per Blazor documentation:
            /// "Blazor WebAssembly apps don't currently have a concept of DI scopes. 
            /// Scoped-registered services behave like Singleton services" 
            builder.Services.AddScoped<DogState>();
            builder.Services.AddScoped<DogProfileState>();
            builder.Services.AddScoped<TemperamentState>();
            builder.Services.AddScoped<BiographyState>();
            builder.Services.AddScoped<DogAlbumState>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<NotificationMsgService>();
            builder.Services.AddScoped<NavigationService>();

            await builder.Build().RunAsync();
        }
    }
}
