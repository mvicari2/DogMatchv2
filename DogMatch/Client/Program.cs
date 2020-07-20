using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Blazor.FileReader;
using DogMatch.Client.Services;
using MatBlazor;
using Radzen;

namespace DogMatch.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("DogMatch.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("DogMatch.ServerAPI"));

            builder.Services.AddApiAuthorization();
            builder.Services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);

            builder.Services.AddSingleton<DogState>();
            builder.Services.AddSingleton<TemperamentState>();
            builder.Services.AddSingleton<NotificationService>();
            builder.Services.AddSingleton<NotificationMsgService>();
            builder.Services.AddSingleton<NavigationService>();

            await builder.Build().RunAsync();
        }
    }
}
