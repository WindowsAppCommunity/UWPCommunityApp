using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using UWPCommLib.Api.UWPComm.Models;

namespace UWPCommLib.Api.UWPComm
{
    [Headers("Authorization: Bearer")]
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
        Task<List<Project>> GetLaunchProjects(int year);

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
        Task PostProject([Body(BodySerializationMethod.UrlEncoded)] NewProjectRequest info);

        /// <summary>
        /// Deletes the project that matches the app name the closest
        /// </summary>
        [Delete("/projects?appName={appName}")]
        Task DeleteProject(string appName);
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
    }
}
