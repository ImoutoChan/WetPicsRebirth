using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class User
    {

        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; } = default!;

        [JsonProperty("name")]
        public string Name { get; set; } = default!;

        [JsonProperty("is_following")]
        public bool? IsFollowing { get; set; }

        [JsonProperty("is_follower")]
        public bool? IsFollower { get; set; }

        [JsonProperty("is_friend")]
        public bool? IsFriend { get; set; }

        [JsonProperty("is_premium")]
        public bool? IsPremium { get; set; }

        [JsonProperty("profile_image_urls")]
        public ProfileImageUrls ProfileImageUrls { get; set; } = default!;

        [JsonProperty("stats")]
        public UserStats Stats { get; set; } = default!;

        [JsonProperty("profile")]
        public Profile Profile { get; set; } = default!;
    }
}
