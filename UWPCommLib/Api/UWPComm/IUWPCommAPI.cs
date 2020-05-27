using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using UWPCommLib.Api.UWPComm.Models;

namespace UWPCommLib.Api.UWPComm
{
    [Headers("Authorization: Bearer", "Origin: uwpcommunity://")]
    public interface IUwpCommApi
    {
        #region /projects/
        /// <summary>
        /// Gets the complete list of registered projects
        /// </summary>
        [Get("/projects")]
        Task<List<Project>> GetProjects();

        /// <summary>
        /// Gets the complete list of projects that are registered for the specified Launch year
        /// </summary>
        [Get("/projects/launch/{year}")]
        Task<LaunchProjects> GetLaunchProjects(int year);

        /// <summary>
        /// Gets the project with the specified ID
        /// </summary>
        [Get("/projects/id/{projectId}")]
        Task<Project> GetProject(int projectId);

        /// <summary>
        /// Gets the list of collaborators for the specified project
        /// </summary>
        [Get("/projects/collaborators?projectId={projectId}")]
        Task<List<Collaborator>> GetProjectCollaborators(string projectId);

        /// <summary>
        /// Registers a project with the given details
        /// </summary>
        [Post("/projects")]
        Task PostProject([Body(BodySerializationMethod.UrlEncoded)] Project info);

        /// <summary>
        /// Updates the project with the given details
        /// </summary>
        [Put("/projects?appName={appName}")]
        Task PutProject(string appName, [Body(BodySerializationMethod.UrlEncoded)] Project info);

        /// <summary>
        /// Deletes the project that matches the app name the closest
        /// </summary>
        [Delete("/projects")]
        Task DeleteProject([Body(BodySerializationMethod.UrlEncoded)] DeleteProjectRequest info);
        #endregion

        #region /user/
        /// <summary>
        /// Gets the user's registered projects (requires authentication)
        /// </summary>
        /// <param name="userId">The Discord ID of the user</param>
        [Get("/user/{userId}/projects")]
        Task<List<Project>> GetUserProjects(string userId);

        /// <summary>
        /// Gets the user's profile information
        /// </summary>
        /// <param name="userId"></param>
        [Get("/user/{userId}")]
        Task<Collaborator> GetUser(string userId);

        /// <summary>
        /// Sets the user's profile information
        /// </summary>
        [Put("/user")]
        Task SetUser([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> newInfo);

        /// <summary>
        /// Creates a user with the specified profile information
        /// </summary>
        [Post("/user")]
        Task PostUser([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> info);
        #endregion

        #region /bot/
        /// <summary>
        /// Gets the user's profile information from Discord
        /// </summary>
        [Get("/bot/user/{userId}")]
        Task<List<Discord.Models.User>> GetDiscordUser(string userId);

        /// <summary>
        /// Gets the user's roles in the UWP Community Discord server
        /// </summary>
        [Get("/bot/user/{userId}/roles")]
        Task<List<Discord.Models.Role>> GetDiscordUserRoles(string userId);
        #endregion
    }
}
