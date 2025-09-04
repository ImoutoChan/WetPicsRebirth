global using MediatR;
global using NodaTime;
global using Telegram.Bot;
global using Telegram.Bot.Types.Enums;
using Flurl.Http.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data;
using WetPicsRebirth.Data.Repositories;
using WetPicsRebirth.Data.Repositories.Abstract;
using WetPicsRebirth.EntryPoint.Service;
using WetPicsRebirth.Extensions;
using WetPicsRebirth.Infrastructure;
using WetPicsRebirth.Infrastructure.Engines;
using WetPicsRebirth.Infrastructure.Engines.Pixiv;
using WetPicsRebirth.Infrastructure.Engines.Pixiv.Models;
using WetPicsRebirth.Infrastructure.ImageProcessing;
using WetPicsRebirth.Jobs;
using WetPicsRebirth.Services;
using WetPicsRebirth.Services.LikesCounterUpdater;
using WetPicsRebirth.Services.UserAccounts;

namespace WetPicsRebirth;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<PixivConfiguration>(Configuration.GetSection("Pixiv"));
        services.Configure<DanbooruConfiguration>(Configuration.GetSection("Danbooru"));

        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "WetPicsRebirth", Version = "v1"});
        });
        services.AddHealthChecks();

        services.AddHttpClient();
        services.AddTransient<ITelegramBotClient>(
            x =>
            {
                var client = x.GetRequiredService<HttpClient>();
                var apiKey = Configuration.GetRequiredValue<string>("TelegramApiKey");

                return new TelegramBotClient(apiKey, client);
            });
        services.AddMemoryCache();

        // services
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IModerationService, ModerationService>();
        services.AddTransient<IAccessControl, AccessControl>();
        services.AddTransient<IScenesRepository, ScenesRepository>();
        services.AddTransient<IActressesRepository, ActressesRepository>();
        services.AddTransient<IPostedMediaRepository, PostedMediaRepository>();
        services.AddTransient<IVotesRepository, VotesRepository>();
        services.AddTransient<IUsersRepository, UsersRepository>();
        services.AddTransient<IModeratedPostsRepository, ModeratedPostsRepository>();
        services.AddTransient<IUserAccountsRepository, CachedUserAccountsRepository>();
        services.AddTransient<UserAccountsRepository, UserAccountsRepository>();

        services.AddTransient<IPopularListLoader, PopularListLoader>();
        services.AddHttpClient<IEngineFactory, EngineFactory>(client =>
            client.DefaultRequestHeaders
                .Add(
                    "User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36"));
        services.AddHttpClient<IPixivApiClient, PixivApiClient>();
        services.AddTransient<IImageSourceApi, ImageSourceApi>();
        services.AddTransient<IPixivAuthorization, PixivAuthorization>();
        services.AddImageProcessing();
        services.AddSingleton<IFlurlClientCache, FlurlClientCache>();

        services.AddLikesToFavoritesOffload();
        services.AddLikesCounterUpdaterOffload();

        services.AddSingleton<IPauseService, PauseService>();

        // mediator
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<Startup>());
        var handlers = GetType().Assembly
            .GetTypes()
            .Where(x => x.IsAssignableTo(typeof(MessageHandler)))
            .Where(x => x != typeof(MessageHandler));
        foreach (var handler in handlers)
        {
            services.AddTransient(handler);
        }

        // data
        services.AddDbContext<WetPicsRebirthDbContext>(
            x => x.UseNpgsql(Configuration.GetRequiredConnectionString("WetPicsRebirthOnPostgres"),
                builder => builder.UseNodaTime()));

        // quartz
        services.AddQuartz(c =>
        {
            c.AddJob<PostingJob>(j => j.WithIdentity(nameof(PostingJob)));
            c.AddTrigger(t => t
                .ForJob(nameof(PostingJob))
                .StartNow()
                .WithSimpleSchedule(b => b.WithIntervalInSeconds(30).RepeatForever()));
            
            c.AddJob<PostWeeklyTopJob>(j => j.WithIdentity(nameof(PostWeeklyTopJob)));
            c.AddTrigger(t => t
                .ForJob(nameof(PostWeeklyTopJob))
                .StartNow()
                .WithCronSchedule("0 00 17 ? * SUN"));

            c.AddJob<PostMonthlyTopJob>(j => j.WithIdentity(nameof(PostMonthlyTopJob)));
            c.AddTrigger(t => t
                .ForJob(nameof(PostMonthlyTopJob))
                .StartNow()
                .WithCronSchedule("0 0 16 L * ? *"));

            c.AddJob<PostYearlyTopJob>(j => j.WithIdentity(nameof(PostYearlyTopJob)));
            c.AddTrigger(t => t
                .ForJob(nameof(PostYearlyTopJob))
                .StartNow()
                .WithCronSchedule("0 0 18 L DEC ? *"));
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WetPicsRebirth v1"));
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }
}
