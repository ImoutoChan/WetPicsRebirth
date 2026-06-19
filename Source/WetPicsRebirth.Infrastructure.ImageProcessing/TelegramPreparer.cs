using NetVips;

namespace WetPicsRebirth.Infrastructure.ImageProcessing;

internal class TelegramPreparer : ITelegramPreparer
{
    // Telegram sendPhoto limits:
    // - the photo must be at most 10 MB in size
    // - the photo's width and height must not exceed 10000 in total
    // - width and height ratio must be at most 20
    private const int PhotoSizeLimit = 1024 * 1024 * 10;
    private const int PhotoDimensionsSumLimit = 10000;
    private const int PhotoSideRatioLimit = 20;

    private const int PhotoDimensionsSumTarget = 9999;

    private static readonly int[] JpegQualitySteps = [95, 92, 90, 85, 80, 75, 65, 55, 45];

    public Stream Prepare(Stream input, long inputByteLength)
    {
        var saveAnyway = inputByteLength >= PhotoSizeLimit;

        using var image = Image.NewFromStream(input);

        EnsureRatioIsValid(image);

        var scale = CalculateScaleFactor(image);
        var shouldScale = scale < 1;

        if (!shouldScale && !saveAnyway)
        {
            input.Seek(0, SeekOrigin.Begin);
            return input;
        }

        if (shouldScale)
        {
            using var resized = image.Resize(scale, Enums.Kernel.Mks2021);
            return EncodeUnderSizeLimit(resized);
        }

        return EncodeUnderSizeLimit(image);
    }

    private static void EnsureRatioIsValid(Image image)
    {
        var ratio = Math.Max(image.Width, image.Height) / (double)Math.Min(image.Width, image.Height);

        if (ratio > PhotoSideRatioLimit)
        {
            throw new InvalidOperationException(
                $"Photo width and height ratio {ratio:F1} exceeds the allowed maximum of {PhotoSideRatioLimit}.");
        }
    }

    private static double CalculateScaleFactor(Image image)
    {
        var dimensionsSum = image.Width + image.Height;

        if (dimensionsSum <= PhotoDimensionsSumLimit)
            return 1;

        return PhotoDimensionsSumTarget / (double)dimensionsSum;
    }

    private static Stream EncodeUnderSizeLimit(Image image)
    {
        foreach (var quality in JpegQualitySteps)
        {
            var output = new MemoryStream();
            image.JpegsaveStream(output, q: quality);

            if (output.Length < PhotoSizeLimit)
            {
                output.Seek(0, SeekOrigin.Begin);
                return output;
            }

            output.Dispose();
        }

        throw new InvalidOperationException(
            $"Unable to fit the photo under {PhotoSizeLimit} bytes even at the lowest jpeg quality.");
    }
}
