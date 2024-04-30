namespace WetPicsRebirth.Services.Offload;

public static class OffloadServiceCollectionExtensions
{
    public static IServiceCollection AddOffload<T>(
        this IServiceCollection services, Action<OffloadOptions<T>> configure)
    {
        services.Configure(configure);

        services.AddSingleton<Offloader<T>>();
        services.AddTransient<IOffloader<T>>(x => x.GetRequiredService<Offloader<T>>());
        services.AddTransient<IOffloadReader<T>>(x => x.GetRequiredService<Offloader<T>>());
        services.AddHostedService<OffloadHostedService<T>>();

        return services;
    }
}
