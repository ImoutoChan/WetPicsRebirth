using System.Net.Http;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Telegram.Bot;
using WetPicsRebirth.EntryPoint.Service;

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

            services.AddTransient<INotificationService, NotificationService>();
            services.AddMediatR(typeof(Startup));
            services.AddMemoryCache();
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
