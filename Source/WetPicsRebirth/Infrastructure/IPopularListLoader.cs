﻿using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure;

/// <summary>
/// Loading tops from boorus and pixiv
/// </summary>
public interface IPopularListLoader
{
    public Task<IReadOnlyCollection<PostHeader>> Load(ImageSource source, string options);

    Task<LoadedPost> LoadPost(ImageSource source, PostHeader header);

    string CreateCaption(ImageSource source, string options, Post post);
}
