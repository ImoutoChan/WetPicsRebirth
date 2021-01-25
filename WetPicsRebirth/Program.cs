using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WetPicsRebirth
{
    public static class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureServices(x => x.AddHostedService<TelegramHostedService>());
    }
}
