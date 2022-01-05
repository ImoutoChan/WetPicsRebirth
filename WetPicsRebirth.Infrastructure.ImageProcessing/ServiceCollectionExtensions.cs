using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("WetPicsRebirth.Tests")]

namespace WetPicsRebirth.Infrastructure.ImageProcessing;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddImageProcessing(this IServiceCollection services)
    {
        services.AddTransient<ITelegramPreparer, TelegramPreparer>();

        return services;
    }
}