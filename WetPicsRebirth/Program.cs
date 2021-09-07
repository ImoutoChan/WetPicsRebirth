using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using WetPicsRebirth.Services;

namespace WetPicsRebirth
{
    public static class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                    builder
                        .AddJsonFile("appsettings.Cache.json.backup", true)
                        .AddJsonFile("appsettings.Cache.json", true))
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureServices(x => x.AddHostedService<TelegramHostedService>())
                .ConfigureServices(x => x.AddQuartzHostedService());
    }
}
