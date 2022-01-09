using Telegram.Bot.Types.ReplyMarkups;

namespace WetPicsRebirth.Services;

public static class Keyboards
{
    public static InlineKeyboardMarkup WithLikes(int likesCount)
        => likesCount > 0
            ? new(InlineKeyboardButton.WithCallbackData($"❤️ {likesCount}", "vote_l"))
            : new(InlineKeyboardButton.WithCallbackData($"❤️", "vote_l"));

    public static InlineKeyboardMarkup WithModeration
        => new(new[]
        {
            InlineKeyboardButton.WithCallbackData("🍒 Approve", "moderate_approve"),
            InlineKeyboardButton.WithCallbackData("💣 Deny", "moderate_deny")
        });
}
