using Refit;
using System.Threading.Tasks;
using UWPCommLib.Api.Discord.Models;

namespace UWPCommLib.Api.Discord
{
    [Headers("Authorization: Bearer")]
    public interface IDiscordApi
    {
        /// <summary>
        /// Gets the user's registered projects (requires authentication)
        /// </summary>
        [Get("/users/@me")]
        Task<User> GetCurrentUser();

        /// <summary>
        /// Gets the public information for the user with the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Get("/users/{id}")]
        Task<User> GetUser(string id);
    }
}
