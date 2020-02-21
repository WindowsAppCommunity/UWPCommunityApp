using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using UWPCommLib.Api.UWPComm.Models;

namespace UWPCommLib.Api.UWPComm
{
    [Headers("Authorization: Bearer")]
    public interface IUwpCommApi
    {
        /// <summary>
        /// Gets the complete list of registered projects
        /// </summary>
        [Get("/projects")]
        Task<List<Project>> GetProjects();
        Task<List<Project>> GetProjects([Authorize] string token);

        /// <summary>
        /// Gets the user's registered projects (requires authentication)
        /// </summary>
        /// <param name="userId">The Discord ID of the user</param>
        [Get("/user/{userId}/projects")]
        Task<List<Project>> GetUserProjects(string userId);
        Task<List<Project>> GetUserProjects(string userId, [Authorize] string token);

        /// <summary>
        /// Gets the complete list of projects that are registered for the specified Launch year
        /// </summary>
        [Get("/projects/{year}")]
        Task<List<Project>> GetLaunchProjects(uint year);
        Task<List<Project>> GetLaunchProjects(uint year, [Authorize] string token);
        Task<List<Project>> GetLaunchProjects(int year);
        Task<List<Project>> GetLaunchProjects(int year, [Authorize] string token);

        /// <summary>
        /// Gets the user's profile information
        /// </summary>
        /// <param name="userId"></param>
        [Get("/user/{userId}")]
        Task<List<Project>> GetUser(string userId);
        Task<List<Project>> GetUser(string userId, [Authorize] string token);
    }
}
