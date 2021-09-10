using Telegram.Bot.Types.ReplyMarkups;

namespace WetPicsRebirth.Services
{
    public static class Keyboards
    {
        public static InlineKeyboardMarkup WithLikes(int likesCount)
            => new(InlineKeyboardButton.WithCallbackData($"❤️ ({likesCount})", "vote_l"));
    }
}
