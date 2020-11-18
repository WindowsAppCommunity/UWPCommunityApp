using Newtonsoft.Json;
using System.Collections.Generic;

namespace UwpCommunityBackend.Models
{
    public class CardInfo
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "subtitle")]
        public string Subtitle { get; set; }

        [JsonProperty(PropertyName = "details")]
        public List<string> Details { get; set; }
    }

    public class CardInfoResponse
    {
        [JsonProperty(PropertyName = "main")]
        public CardInfo Main { get; set; }
    }
}
