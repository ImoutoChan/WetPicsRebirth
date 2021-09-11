using System.IO;

namespace WetPicsRebirth.Infrastructure.ImageProcessing
{
    public interface ITelegramPreparer
    {
        Stream Prepare(Stream input, long inputByteLength);
    }
}
