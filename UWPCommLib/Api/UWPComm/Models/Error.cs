using Newtonsoft.Json;

namespace UWPCommLib.Api.UWPComm.Models
{
    public class Error
    {
        [JsonProperty(PropertyName = "error")]
        public string ErrorText { get; set; }

        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }
    }
}
