using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(DogMatch.Server.Areas.Identity.IdentityHostingStartup))]
namespace DogMatch.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}