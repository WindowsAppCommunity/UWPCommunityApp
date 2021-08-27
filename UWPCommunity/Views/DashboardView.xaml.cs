using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UwpCommunityBackend.Models;
using UWPCommunity.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UwpCommunityBackend;
using System.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardView : Page
    {
        public DashboardView()
        {
            InitializeComponent();
            Loaded += DashboardView_Loaded;
        }

        private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserManager.IsLoggedIn)
            {
                NavigationManager.RequestSignIn(typeof(DashboardView));
                return;
            }

            List<Discord.Models.Guild> guilds = null;
            try
            {
                guilds = await Discord.Api.GetCurrentUserGuilds();
            }
            catch (Flurl.Http.FlurlHttpException ex)
            {
                if (ex.Message.Contains("429"))
                {
                    // HTTP error 429 is Too Many Requests. Wait and try again.
                    await System.Threading.Tasks.Task.Delay(1000);
                    guilds = await Discord.Api.GetCurrentUserGuilds();
                }
            }
            bool isInServer = guilds?.Any(g => g.Id == "372137812037730304") ?? false;
            if (false)//!isInServer)
            {
                NavigationManager.Navigate(typeof(NewAccount.JoinServerView));
            }

            try
            {
                RefreshProjects();
                RefreshUser();

                var roles = await Api.GetDiscordUserRoles(UserManager.DiscordUser.DiscordId);
                //var member = await Discord.Api.GetGuildMember(Common.DISCORD_GUILD_ID, UserManager.DiscordUser.DiscordId);
                if (roles.Any(r => r.Id == Api.SpecialRoles["Developer"]))
                {
                    // User is a developer, set the buttons accordingly
                    BecomeDeveloperButton.Visibility = Visibility.Collapsed;
                    RegisterAppButton.Visibility = Visibility.Visible;
                }
                else
                {
                    // User is NOT a developer, set the buttons accordingly
                    BecomeDeveloperButton.Visibility = Visibility.Visible;
                    RegisterAppButton.Visibility = Visibility.Collapsed;
                }
            }
            catch (Flurl.Http.FlurlHttpException ex)
            {
                Debug.WriteLine("API Exception:\n" + await ex.GetResponseStringAsync());
            }

        }

        private async void RefreshProjects()
        {
            try
            {
                ViewModel.AllProjects?.Clear();
                var projs = await Api.GetUserProjects(UserManager.DiscordUser.DiscordId);
                ViewModel.AllProjects = projs.Select(p => new ProjectViewModel(p)).ToList();
            }
            catch (Flurl.Http.FlurlHttpException ex)
            {
                if (ex.Call.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // This means something went wrong with authentication,
                    // so attempt to log in again.
                    // TODO: Maybe this can be solved with refresh tokens?
                    UserManager.SignOut();
                    NavigationManager.RequestSignIn(typeof(DashboardView));
                }
            }
        }

        private async void RefreshUser()
        {
            var discordUser = UserManager.DiscordUser;
            UserProfilePicture.ProfilePicture =
                new Windows.UI.Xaml.Media.Imaging.BitmapImage(discordUser.AvatarUri);
            UserProfileUsername.Text = discordUser.Username + "#" + discordUser.Discriminator;

            var user = await UserManager.GetCurrentUser();
            UserProfileName.Text = user.Name;
            UserProfileEmail.Text = user.Email;
        }

        private void RegisterAppButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Dashboard: App registration started");
            NavigationManager.Navigate(typeof(Subviews.EditProjectView));
        }

        private void Project_EditRequested(object p)
        {
            NavigationManager.NavigateToEditProject(p);
            RefreshProjects();
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Dashboard: Edit project",
                new Dictionary<string, string> {
                    { "Proj", (p as ProjectViewModel).Project.Id.ToString() },
                }
            );
        }

        private async void Project_DeleteRequested(object p)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Are you sure?",
                Content = "This action cannot be undone",
                PrimaryButtonText = "Yes, delete",
                SecondaryButtonText = "Cancel",
                RequestedTheme = SettingsManager.GetAppTheme()
            };
            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    await Api.DeleteProject(
                        new DeleteProjectRequest((p as ProjectViewModel).Project.AppName));
                    RefreshProjects();
                    Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Dashboard: Delete project",
                        new Dictionary<string, string> {
                            { "Proj", (p as ProjectViewModel).Project.Id.ToString() },
                        }
                    );
                }
                catch (Flurl.Http.FlurlHttpException ex)
                {
                    var error = await ex.GetResponseJsonAsync<Error>();
                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Failed to delete project",
                        Content = error.Reason,
                        CloseButtonText = "Ok",
                        RequestedTheme = SettingsManager.GetAppTheme()
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private void Project_ViewRequested(object p)
        {
            Project proj = (p as ProjectViewModel).Project;
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Dashboard: View project",
                new Dictionary<string, string> {
                    { "Proj", proj.Id.ToString() },
                }
            );
            NavigationManager.NavigateToViewProject(proj);
        }

        private void ProjectsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Project_ViewRequested(e.ClickedItem);
        }

        private void RefreshContainer_RefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            RefreshProjects();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Dashboard: Navigated to",
                new Dictionary<string, string> {
                    { "From", e.SourcePageType.Name },
                    { "Parameters", e.Parameter?.ToString() }
                }
            );
        }

        private void BecomeDeveloperButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Dialogs.EditProfileDialog();
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                // User likely updated profile, update UI to reflect changes
                RefreshUser();
            }
        }
    }
}
