using System.Linq;
using System.Net.Http;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Quartz;
using Telegram.Bot;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data;
using WetPicsRebirth.Data.Repositories;
using WetPicsRebirth.EntryPoint.Service;
using WetPicsRebirth.Infrastructure;
using WetPicsRebirth.Infrastructure.Engines;
using WetPicsRebirth.Infrastructure.Engines.Pixiv;
using WetPicsRebirth.Infrastructure.Engines.Pixiv.Models;
using WetPicsRebirth.Infrastructure.ImageProcessing;
using WetPicsRebirth.Jobs;
using WetPicsRebirth.Services;

namespace WetPicsRebirth
{
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

            services.AddControllers().AddNewtonsoftJson();
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
                    var apiKey = Configuration.GetValue<string>("TelegramApiKey");

                    return new TelegramBotClient(apiKey, client);
                });
            services.AddMemoryCache();

            // services
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IAccessControl, AccessControl>();
            services.AddTransient<IScenesRepository, ScenesRepository>();
            services.AddTransient<IActressesRepository, ActressesRepository>();
            services.AddTransient<IPostedMediaRepository, PostedMediaRepository>();
            services.AddTransient<IVotesRepository, VotesRepository>();
            services.AddTransient<IUsersRepository, UsersRepository>();

            services.AddTransient<IPopularListLoader, PopularListLoader>();
            services.AddHttpClient<IEngineFactory, EngineFactory>();
            services.AddHttpClient<IPixivApiClient, PixivApiClient>();
            services.AddTransient<IPixivAuthorization, PixivAuthorization>();
            services.AddImageProcessing();

            // mediator
            services.AddMediatR(typeof(Startup));
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
                x => x.UseNpgsql(Configuration.GetConnectionString("WetPicsRebirthOnPostgres")));

            // quartz
            services.AddQuartz(c =>
            {
                c.UseMicrosoftDependencyInjectionJobFactory();
                c.AddJob<PostingJob>(j => j.WithIdentity(nameof(PostingJob)));
                c.AddTrigger(t => t
                    .ForJob(nameof(PostingJob))
                    .StartNow()
                    .WithSimpleSchedule(b => b.WithIntervalInSeconds(30).RepeatForever()));
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
}
