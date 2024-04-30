namespace WetPicsRebirth.Services.Offload;

public class OffloadOptions<T>
{
    public required Func<IServiceProvider, T, Task> ItemProcessor { get; set; }
    
    public required Action<ILogger, T, Exception> ErrorLogger { get; set; }
}
