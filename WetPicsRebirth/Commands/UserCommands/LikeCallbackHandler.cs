using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories;
using WetPicsRebirth.EntryPoint.Service.Notifications;
using WetPicsRebirth.Services;

namespace WetPicsRebirth.Commands.UserCommands
{
    public class LikeCallbackHandler : ICallbackHandler
    {
        private const string LikeData = "vote_l";
        private readonly ITelegramBotClient _telegramBotClient;

        private readonly IUsersRepository _usersRepository;
        private readonly IVotesRepository _votesRepository;

        public LikeCallbackHandler(
            IUsersRepository usersRepository,
            IVotesRepository votesRepository,
            ITelegramBotClient telegramBotClient)
        {
            _usersRepository = usersRepository;
            _votesRepository = votesRepository;
            _telegramBotClient = telegramBotClient;
        }

        public async Task Handle(CallbackNotification notification, CancellationToken token)
        {
            if (notification.CallbackQuery.Data != LikeData)
                return;

            var messageId = notification.CallbackQuery.Message.MessageId;
            var chatId = notification.CallbackQuery.Message.Chat.Id;
            var userId = notification.CallbackQuery.From.Id;

            var vote = new Vote(userId, chatId, messageId);
            var user = new User
            {
                Id = notification.CallbackQuery.From.Id,
                FirstName = notification.CallbackQuery.From.FirstName,
                LastName = notification.CallbackQuery.From.LastName,
                Username = notification.CallbackQuery.From.Username
            };

            await _usersRepository.AddOrUpdate(user);
            var counts = await _votesRepository.AddOrRemove(vote);

            await _telegramBotClient.AnswerCallbackQueryAsync(notification.CallbackQuery.Id, cancellationToken: token);
            await _telegramBotClient.EditMessageReplyMarkupAsync(chatId, messageId, Keyboards.WithLikes(counts), token);
        }
    }
}
