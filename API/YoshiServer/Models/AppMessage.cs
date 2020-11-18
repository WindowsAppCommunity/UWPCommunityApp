using Newtonsoft.Json;

namespace YoshiServer.Models
{
    public class AppMessage
    {
        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// The importance of this message. 1 is highest, 3 is lowest
        /// </summary>
        [JsonProperty(PropertyName = "importance")]
        public int Importance { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
