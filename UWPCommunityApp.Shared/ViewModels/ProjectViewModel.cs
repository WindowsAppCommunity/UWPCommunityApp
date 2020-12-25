using UwpCommunityBackend.Models;

namespace UWPCommunityApp.ViewModels
{
    public class ProjectViewModel
    {
        public Project project;

        public ProjectViewModel(Project project)
        {
            this.project = project;
        }

        public bool IsOwner {
            get
			{
                if (UserManager.DiscordUser == null)
                    return false;
                return project.IsOwner(UserManager.DiscordUser.DiscordId);
            }
        }
        public bool IsDeveloper
        {
            get
            {
                if (UserManager.DiscordUser == null)
                    return false;
                return project.IsDeveloper(UserManager.DiscordUser.DiscordId);
            }
        }
        public bool IsTranslator
        {
            get
            {
                if (UserManager.DiscordUser == null)
                    return false;
                return project.IsTranslator(UserManager.DiscordUser.DiscordId);
            }
        }
        public bool IsBetaTester
        {
            get
            {
                if (UserManager.DiscordUser == null)
                    return false;
                return project.IsBetaTester(UserManager.DiscordUser.DiscordId);
            }
        }
    }
}
