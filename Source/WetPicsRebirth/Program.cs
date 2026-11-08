using Microsoft.EntityFrameworkCore;
using Quartz;
using WetPicsRebirth.Data;
using WetPicsRebirth.Extensions;
using WetPicsRebirth.Services;

namespace WetPicsRebirth;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await MigrateAsync(host);
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, builder) => builder.AddPixivTokenCache())
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .ConfigureSerilog()
            .ConfigureServices(x => x.AddHostedService<TelegramHostedService>())
            .ConfigureServices(x => x.AddQuartzHostedService());


    private static async Task MigrateAsync(IHost app)
    {
        var logger = app.Services.GetRequiredService<ILogger<WetPicsRebirthDbContext>>();

        try
        {
            using var serviceScope = app.Services.CreateScope();
            await using var db = serviceScope.ServiceProvider.GetRequiredService<WetPicsRebirthDbContext>();

            await db.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occurred in migration process");
            throw;
        }
    }
}
