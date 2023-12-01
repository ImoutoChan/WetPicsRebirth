using NetVips;

namespace WetPicsRebirth.Infrastructure.ImageProcessing;

internal class TelegramPreparer : ITelegramPreparer
{
    private const int PhotoSizeLimit = 1024 * 1024 * 5;
    private const int PhotoHeightLimit = 2560;
    private const int PhotoWidthLimit = 2560;

    public Stream Prepare(Stream input, long inputByteLength)
    {
        var saveAnyway = inputByteLength >= PhotoSizeLimit;

        using var image = Image.NewFromStream(input);
        var scale = CalculateScaleFactor(image);
        var shouldScale = scale < 1;

        if (shouldScale || saveAnyway)
        {
            using var resized = image.Resize(scale, Enums.Kernel.Cubic);

            var output = new MemoryStream();
            resized.JpegsaveStream(output);
            output.Seek(0, SeekOrigin.Begin);

            return output;
        }

        input.Seek(0, SeekOrigin.Begin);
        return input;
    }

    private static double CalculateScaleFactor(Image image)
    {
        double minRatio = 1;

        var imageTooBig = image.Height - PhotoHeightLimit >= 0 || image.Width - PhotoWidthLimit >= 0;

        if (imageTooBig)
        {
            double ratioH = PhotoHeightLimit / (double)image.Height;
            double ratioW = PhotoWidthLimit / (double)image.Width;

            ratioH = Math.Min(1, ratioH);
            ratioW = Math.Min(1, ratioW);

            minRatio = Math.Min(ratioW, ratioH);
        }

        return minRatio;
    }
}
