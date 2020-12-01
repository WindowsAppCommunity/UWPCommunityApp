using Newtonsoft.Json;

namespace UwpCommunityBackend.Models
{
    public class Error
    {
        [JsonProperty(PropertyName = "error")]
        public string ErrorText { get; set; }

        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }
    }

    public class TokenResponseError
    {
        [JsonProperty(PropertyName = "error")]
        public string ErrorText { get; set; }

        [JsonProperty(PropertyName = "error_description")]
        public string ErrorDescription { get; set; }
    }
}
