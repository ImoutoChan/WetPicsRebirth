using WetPicsRebirth.Infrastructure.ImageProcessing;
using Xunit;

namespace WetPicsRebirth.Tests;

public class TelegramPreparerTests
{
    private readonly ITelegramPreparer _telegramPreparer;

    public TelegramPreparerTests()
    {
        _telegramPreparer = new TelegramPreparer();
    }

    [Fact]
    public async Task TelegramPreparerShouldResizeImage()
    {
        var input = new FileInfo("TestSubjects\\5777e304cc7f88558e769ede3d61c703.png");
        var output = new FileInfo("TestSubjects\\resized.png");

        if (output.Exists)
            output.Delete();

        await _telegramPreparer.Prepare(input.OpenRead(), input.Length).CopyToAsync(output.OpenWrite());
    }
}