using System.Threading.Channels;

namespace WetPicsRebirth.Services.Offload;

internal interface IOffloadReader<T>
{
    ChannelReader<T> Reader { get; }
    
    void Complete();
}
