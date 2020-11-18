using UwpCommunityBackend.Models;

namespace UWPCommunity.ViewModels
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
                if (Common.DiscordUser == null)
                    return false;
                return project.IsOwner(Common.DiscordUser.DiscordId);
            }
        }
        public bool IsDeveloper
        {
            get
            {
                if (Common.DiscordUser == null)
                    return false;
                return project.IsDeveloper(Common.DiscordUser.DiscordId);
            }
        }
        public bool IsTranslator
        {
            get
            {
                if (Common.DiscordUser == null)
                    return false;
                return project.IsTranslator(Common.DiscordUser.DiscordId);
            }
        }
        public bool IsBetaTester
        {
            get
            {
                if (Common.DiscordUser == null)
                    return false;
                return project.IsBetaTester(Common.DiscordUser.DiscordId);
            }
        }
    }
}
