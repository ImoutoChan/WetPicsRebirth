using Microsoft.Extensions.Options;

namespace WetPicsRebirth.Services.Offload;

internal class OffloadHostedService<T> : IHostedService
{
    private readonly IOffloadReader<T> _offload;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<OffloadOptions<T>> _options;
    private readonly ILogger<OffloadHostedService<T>> _logger;

    protected OffloadHostedService(
        IOffloadReader<T> offload,
        IServiceProvider serviceProvider,
        IOptions<OffloadOptions<T>> options,
        ILogger<OffloadHostedService<T>> logger)
    {
        _offload = offload;
        _serviceProvider = serviceProvider;
        _options = options;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(Process, cancellationToken);

        return Task.CompletedTask;
    }

    private async Task Process()
    {
        var reader = _offload.Reader;

        while (await reader.WaitToReadAsync())
        while (reader.TryRead(out var item))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                await _options.Value.ItemProcessor(scope.ServiceProvider, item);
            }
            catch (Exception e)
            {
                _options.Value.ErrorLogger(_logger, item, e);
            }
        }
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _offload.Complete();
        return Task.CompletedTask;
    }
}
