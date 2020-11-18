using Discord.Models;
using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace Discord
{
    public static class Api
    {
        public const string BASE_URL = "https://discordapp.com/api";

        public static string Token { get; set; }

        /// <summary>
        /// Gets the user's registered projects (requires authentication)
        /// </summary>
        public static async Task<User> GetCurrentUser()
        {
            return await BASE_URL.AppendPathSegments("users", "@me")
                .WithOAuthBearerToken(Token)
                .GetJsonAsync<User>();
        }

        /// <summary>
        /// Gets the public information for the user with the given ID
        /// </summary>
        /// <param name="id"></param>
        public static async Task<User> GetUser(string id)
        {
            return await BASE_URL.AppendPathSegments("users", id)
                .WithOAuthBearerToken(Token)
                .GetJsonAsync<User>();
        }
    }
}
