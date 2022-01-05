﻿using System;
using System.IO;
using NetVips;

namespace WetPicsRebirth.Infrastructure.ImageProcessing;

internal class TelegramPreparer : ITelegramPreparer
{
    private static readonly int _photoSizeLimit = 1024 * 1024 * 5;
    private static readonly int _photoHeightLimit = 2560;
    private static readonly int _photoWidthLimit = 2560;

    public Stream Prepare(Stream input, long inputByteLength)
    {
        var saveAnyway = inputByteLength >= _photoSizeLimit;

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

        if (image.Height - _photoHeightLimit >= 0 || image.Width - _photoWidthLimit >= 0)
        {
            double ratioH = _photoHeightLimit / (double)image.Height;
            double ratioW = _photoWidthLimit / (double)image.Width;

            ratioH = Math.Min(1, ratioH);
            ratioW = Math.Min(1, ratioW);

            minRatio = Math.Min(ratioW, ratioH);
        }

        return minRatio;
    }
}