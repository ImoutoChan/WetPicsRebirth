using Microsoft.EntityFrameworkCore;
using Quartz;
using WetPicsRebirth.Data;
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
            .ConfigureAppConfiguration((context, builder) =>
            {
                var azureAppConfiguration = builder.Build().GetConnectionString("AzureApplicationConfiguration");

                if (context.HostingEnvironment.IsProduction())
                {
                    builder.AddAzureAppConfiguration(x => x
                        .Connect(azureAppConfiguration)
                        .Select("*", Environments.Production));
                }

                builder
                    .AddJsonFile("appsettings.Cache.json.backup", true)
                    .AddJsonFile("appsettings.Cache.json", true);
            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
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
