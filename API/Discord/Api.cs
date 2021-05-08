using Discord.Models;
using Flurl;
using Flurl.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public static class Api
    {
        public const string BASE_URL = "https://discordapp.com/api";

        public static string Token { get; set; }
        public static string RefreshToken { get; set; }

        /// <summary>
        /// Returns the <see cref="User"/> object of the requester's account.
        /// For OAuth2, this requires the identify scope, which will return the object
        /// without an email, and optionally the email scope, which returns the object
        /// with an email.
        /// </summary>
        public static async Task<User> GetCurrentUser()
        {
            return await BASE_URL.AppendPathSegments("users", "@me")
                .WithOAuthBearerToken(Token)
                .GetJsonAsync<User>();
        }

        /// <summary>
        /// Returns a list of partial <see cref="Guild"/> objects the current user is a member of.
        /// Requires the guilds OAuth2 scope.
        /// </summary>
        public static async Task<List<Guild>> GetCurrentUserGuilds()
        {
            return await BASE_URL.AppendPathSegments("users", "@me", "guilds")
                .WithOAuthBearerToken(Token)
                .GetJsonAsync<List<Guild>>();
        }

        /// <summary>
        /// Returns a <see cref="User"/> object for a given user ID.
        /// </summary>
        public static async Task<User> GetUser(string id)
        {
            return await BASE_URL.AppendPathSegments("users", id)
                .WithOAuthBearerToken(Token)
                .GetJsonAsync<User>();
        }

        /// <summary>
        /// Returns a <see cref="GuildMember"/> object for the specified user.
        /// </summary>
        public static async Task<GuildMember> GetGuildMember(string guildId, string userId)
        {
            return await BASE_URL.AppendPathSegments("guilds", guildId, "members", userId)
                .WithOAuthBearerToken(Token)
                .GetJsonAsync<GuildMember>();
        }
    }
}
