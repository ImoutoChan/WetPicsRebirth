using Telegram.Bot.Types.ReplyMarkups;

namespace WetPicsRebirth.Services
{
    public static class Keyboards
    {
        public static InlineKeyboardMarkup WithLikes(int likesCount)
            => likesCount > 0
                ? new(InlineKeyboardButton.WithCallbackData($"❤️ ({likesCount})", "vote_l"))
                : new(InlineKeyboardButton.WithCallbackData($"❤️", "vote_l"));
    }
}
