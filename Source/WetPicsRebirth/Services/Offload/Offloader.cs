using System.Threading.Channels;

namespace WetPicsRebirth.Services.Offload;

/// <remarks>
/// Should be registered as a singleton.
/// </remarks>>
internal class Offloader<T> : IOffloader<T>, IOffloadReader<T>
{
    private Channel<T> VotesToTranslate { get; } = Channel.CreateUnbounded<T>();

    public async Task Offload(T vote) => await VotesToTranslate.Writer.WriteAsync(vote);

    public ChannelReader<T> Reader => VotesToTranslate.Reader;
    
    public void Complete() => VotesToTranslate.Writer.Complete();
}
