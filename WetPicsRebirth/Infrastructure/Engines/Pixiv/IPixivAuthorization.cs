﻿using System.Threading.Tasks;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv
{
    public interface IPixivAuthorization
    {
        Task<string> GetAccessToken();

        void ResetAccessToken();
    }
}
