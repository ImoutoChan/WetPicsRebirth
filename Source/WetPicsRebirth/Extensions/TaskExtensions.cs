namespace WetPicsRebirth.Extensions;

public static class TaskExtensions
{
    public static async Task TryRun(this Task taskToRun, ILogger? logger = null)
    {
        try
        {
            await taskToRun;
        }
        catch (Exception e)
        {
            logger?.LogWarning(e, "Unable to perform an operation");
        }
    }
}
