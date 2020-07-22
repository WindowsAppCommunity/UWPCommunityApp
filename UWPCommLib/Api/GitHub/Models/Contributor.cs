using Newtonsoft.Json;

namespace UWPCommLib.Api.GitHub.Models
{
    public class Contributor : UserBase
    {
        [JsonProperty(PropertyName = "contributions")]
        public int Contributions { get; set; }
    }
}
