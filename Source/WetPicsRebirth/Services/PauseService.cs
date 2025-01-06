using System.Collections.Concurrent;
using WetPicsRebirth.Commands.ServiceCommands.Posting;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Services;

/// <summary>
/// Stateful, should be registered as a singleton.
/// </summary>
public class PauseService : IPauseService
{
    private readonly ConcurrentDictionary<long, Instant> _pausedChatIds = new();

    public bool IsPaused(Scene scene)
        => _pausedChatIds.TryGetValue(scene.ChatId, out var pausedUntil)
           && pausedUntil > SystemClock.Instance.GetCurrentInstant();

    public void Pause(Scene scene, TopType topType)
    {
        var pauseTime = GetPauseTime(topType);
        var now = SystemClock.Instance.GetCurrentInstant();
        var pauseUntil = now.Plus(pauseTime);
        _pausedChatIds.AddOrUpdate(scene.ChatId, _ => pauseUntil, (_, _) => pauseUntil);
    }

    private static Duration GetPauseTime(TopType topType)
        => topType switch
        {
            TopType.Weekly => Duration.FromHours(2),
            TopType.Monthly => Duration.FromHours(6),
            TopType.Yearly => Duration.FromHours(24),
            _ => throw new ArgumentOutOfRangeException(nameof(topType), topType, null)
        };
}
