using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace UWPCommLib.Api.Discord.Models
{
    public class TokenRequest
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("grant_type")]
        public string GrantType { get; set; }

        [JsonProperty("code")]
        public string AuthorizationCode { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
