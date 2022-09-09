﻿using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories.Abstract;

public interface IVotesRepository
{
    Task<int> AddOrIgnore(Vote vote);
    
    Task<IReadOnlyCollection<PostedMedia>> GetTopForWeek(int count);
}