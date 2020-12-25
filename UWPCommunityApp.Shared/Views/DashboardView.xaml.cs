using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UwpCommunityBackend.Models;
using UWPCommunityApp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UwpCommunityBackend;
using System.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunityApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardView : Page
    {
        public ObservableCollection<ProjectViewModel> Projects { get; set; } = new ObservableCollection<ProjectViewModel>();
        public DashboardView()
        {
            this.InitializeComponent();
            Loaded += DashboardView_Loaded;
        }

        private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            var cardSize = SettingsManager.GetProjectCardSize();
            ProjectsGridView.DesiredWidth = cardSize.X;
            ProjectsGridView.ItemHeight = cardSize.Y;

            if (!UserManager.IsLoggedIn)
            {
                NavigationManager.RequestSignIn(typeof(DashboardView));
                return;
            }

            bool isInServer = (await Discord.Api.GetCurrentUserGuilds()).Any(g => g.Id == "372137812037730304");
            if (!isInServer)
            {
                NavigationManager.Navigate(typeof(NewAccount.JoinServerView));
            }

            try
            {
                RefreshProjects();
                UserProfilePicture.ProfilePicture =
                    new Windows.UI.Xaml.Media.Imaging.BitmapImage(UserManager.DiscordUser.AvatarUri);
                UserProfileUsername.Text = UserManager.DiscordUser.Username;
            }
            catch (Flurl.Http.FlurlHttpException ex)
            {
                System.Diagnostics.Debug.WriteLine("API Exception:\n" + await ex.GetResponseStringAsync());
            }

        }

        private async void RefreshProjects()
        {
            Projects.Clear();
            try
            {
                var projs = await Api.GetUserProjects(UserManager.DiscordUser.DiscordId);
                foreach (var project in projs)
                {
                    Projects.Add(new ProjectViewModel(project));
                }
            }
            catch (Flurl.Http.FlurlHttpException ex)
            {
                if (ex.Call.Response.StatusCode == 401)
                {
                    // This means something went wrong with authentication,
                    // so attempt to log in again.
                    // TODO: Maybe this can be solved with refresh tokens?
                    UserManager.SignOut();
                    NavigationManager.RequestSignIn(typeof(DashboardView));
                }
            }
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
                    { "Proj", (p as ProjectViewModel).project.Id.ToString() },
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
                        new DeleteProjectRequest((p as ProjectViewModel).project.AppName));
                    RefreshProjects();
                    Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Dashboard: Delete project",
                        new Dictionary<string, string> {
                            { "Proj", (p as ProjectViewModel).project.Id.ToString() },
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
            Project proj = (p as ProjectViewModel).project;
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

        private void RefreshContainer_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
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
    }
}
