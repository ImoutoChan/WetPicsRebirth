using System.IO;

namespace WetPicsRebirth.Infrastructure.Engines
{
    public record MeasuredStream(Stream Stream, long Length);
}
