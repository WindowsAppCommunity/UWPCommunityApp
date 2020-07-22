using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWPCommLib.Api.GitHub.Models;

namespace UWPCommLib.Api.GitHub
{
    [Headers("User-Agent: UWP Community App")]
    public interface IGitHubApi
    {
        /// <summary>
        /// Gets the list of all collaborators for the sepcified repo
        /// </summary>
        [Get("/repos/{owner}/{repo}/contributors")]
        Task<List<Contributor>> GetContributors(string owner, string repo);

        /// <summary>
        /// Gets specified user
        /// </summary>
        [Get("/users/{user}")]
        Task<GitHubUser> GetUser(string user);
    }
}
