using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace FoundationCore.Web.Models.Search
{
    public class SearchItem
    {
        [JsonPropertyName("Results")]
        public string Results { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("subtitle")]
        public string Subtitle { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonProperty("image_url")]
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("more_url")]
        [JsonPropertyName("more_url")]
        public string MoreUrl { get; set; }

        [JsonProperty("tag")]
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonProperty("size")]
        [JsonPropertyName("size")]
        public string FileSize { get; set; }

        [JsonProperty("format")]
        [JsonPropertyName("format")]
        public string FileFormat { get; set; }
    }
}
