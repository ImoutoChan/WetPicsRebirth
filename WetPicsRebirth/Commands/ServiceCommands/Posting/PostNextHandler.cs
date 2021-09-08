using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories;
using WetPicsRebirth.Infrastructure;

namespace WetPicsRebirth.Commands.ServiceCommands.Posting
{
    public class PostNextHandler : IRequestHandler<PostNext>
    {
        private static readonly ConcurrentDictionary<long, int> ScenePostNumbers = new();

        private readonly IActressesRepository _actressesRepository;
        private readonly ILogger<PostNextHandler> _logger;
        private readonly IScenesRepository _scenesRepository;
        private readonly IPopularListLoader _popularListLoader;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IPostedMediaRepository _postedMediaRepository;
        private readonly string _channelLink;
        private readonly string _accessLink;

        public PostNextHandler(
            IScenesRepository scenesRepository,
            IActressesRepository actressesRepository,
            ILogger<PostNextHandler> logger,
            IPopularListLoader popularListLoader,
            ITelegramBotClient telegramBotClient,
            IPostedMediaRepository postedMediaRepository,
            IConfiguration configuration)
        {
            _scenesRepository = scenesRepository;
            _actressesRepository = actressesRepository;
            _logger = logger;
            _popularListLoader = popularListLoader;
            _telegramBotClient = telegramBotClient;
            _postedMediaRepository = postedMediaRepository;
            _channelLink = configuration.GetValue<string>("ChannelInviteLink");
            _accessLink = configuration.GetValue<string>("AccessLink");
        }

        public async Task<Unit> Handle(PostNext request, CancellationToken cancellationToken)
        {
            var readyScenes = await _scenesRepository.GetEnabledAndReady();

            foreach (var scene in readyScenes)
            {
                var actresses = await _actressesRepository.GetForChat(scene.ChatId);

                if (actresses.Count == 0)
                    continue;

                await PostNextForScene(scene, actresses);
            }

            return Unit.Value;
        }

        private async Task PostNextForScene(Scene scene, IReadOnlyCollection<Actress> actresses)
        {
            for (int i = 0; i < actresses.Count; i++)
            {
                var now = DateTimeOffset.Now;
                var postNumber = GetScenePostNumber(scene.ChatId);
                var selectedActress = actresses.ElementAt(postNumber % actresses.Count);

                try
                {
                    await PostNextForActress(selectedActress);
                    await _scenesRepository.SetPostedAt(scene.ChatId, now);
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        e,
                        "Unable to post for actress {ImageSource} {ChatId}",
                        selectedActress.ImageSource,
                        selectedActress.ChatId);
                }
            }
        }

        private async Task PostNextForActress(Actress actress)
        {
            var list = await _popularListLoader.Load(actress.ImageSource, actress.Options);
            var postIds = list.Select(x => x.Id).ToList();

            var newId = await _postedMediaRepository.GetFirstNew(actress.ChatId, actress.ImageSource, postIds);

            if (newId == null)
            {
                throw new Exception("No new images in actress");
            }

            var post = await _popularListLoader.LoadPost(actress.ImageSource, list.First(x => x.Id == newId.Value));
            var caption = _popularListLoader.CreateCaption(actress.ImageSource, actress.Options, post);
            caption = EnrichCaption(caption);

            var sentPost = await _telegramBotClient.SendPhotoAsync(
                actress.ChatId,
                new InputOnlineFile(post.File),
                caption,
                ParseMode.Html);

            var fileId = sentPost.Photo
                .OrderByDescending(x => x.Height)
                .ThenByDescending(x => x.Width)
                .Select(x => x.FileId)
                .First();

            await _postedMediaRepository.Add(
                actress.ChatId,
                sentPost.MessageId,
                fileId,
                actress.ImageSource,
                post.PostHeader.Id);
        }

        private string EnrichCaption(string caption)
            => $"{caption}\n<a href=\"{_channelLink}\">join</a> | <a href=\"{_accessLink}\">access</a>";

        private static int GetScenePostNumber(long sceneId)
        {
            ScenePostNumbers.AddOrUpdate(sceneId, 0, (_, old) => old + 1);
            return ScenePostNumbers[sceneId];
        }
    }
}
