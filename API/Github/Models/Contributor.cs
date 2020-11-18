using Newtonsoft.Json;

namespace GitHub.Models
{
    public class Contributor : UserBase
    {
        [JsonProperty(PropertyName = "contributions")]
        public int Contributions { get; set; }
    }
}
