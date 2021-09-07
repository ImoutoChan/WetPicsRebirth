using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class Work
    {

        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; } = default!;

        [JsonProperty("caption")]
        public string Caption { get; set; } = default!;

        [JsonProperty("tags")]
        public IList<string> Tags { get; set; } = default!;

        [JsonProperty("tools")]
        public IList<string> Tools { get; set; } = default!;

        [JsonProperty("image_urls")]
        public ImageUrls ImageUrls { get; set; } = default!;

        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }

        [JsonProperty("stats")]
        public WorkStats Stats { get; set; } = default!;

        [JsonProperty("publicity")]
        public int? Publicity { get; set; }

        [JsonProperty("age_limit")]
        public string AgeLimit { get; set; } = default!;

        [JsonProperty("created_time")]
        public DateTimeOffset CreatedTime { get; set; }

        [JsonProperty("reuploaded_time")]
        public string ReuploadedTime { get; set; } = default!;

        [JsonProperty("user")]
        public User User { get; set; } = default!;

        [JsonProperty("is_manga")]
        public bool? IsManga { get; set; }

        [JsonProperty("is_liked")]
        public bool? IsLiked { get; set; }

        [JsonProperty("favorite_id")]
        public long? FavoriteId { get; set; }

        [JsonProperty("page_count")]
        public int? PageCount { get; set; }

        [JsonProperty("book_style")]
        public string BookStyle { get; set; } = default!;

        [JsonProperty("type")]
        public string Type { get; set; } = default!;

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; } = default!;

        [JsonProperty("content_type")]
        public string ContentType { get; set; } = default!;
    }
}
