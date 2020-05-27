using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UWPCommLib.Api.UWPComm.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardView : Page
    {
        public ObservableCollection<Project> Projects { get; set; } = new ObservableCollection<Project>();
        public DashboardView()
        {
            this.InitializeComponent();
            Loaded += DashboardView_Loaded;
        }

        private void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            var cardSize = SettingsManager.GetProjectCardSize();
            ProjectsGridView.DesiredWidth = cardSize.X;
            ProjectsGridView.ItemHeight = cardSize.Y;

            if (!Common.IsLoggedIn)
            {
                NavigationManager.RequestSignIn(typeof(DashboardView));
                return;
            }

            try
            {
                RefreshProjects();
                UserProfilePicture.ProfilePicture =
                    new Windows.UI.Xaml.Media.Imaging.BitmapImage(Common.DiscordUser.AvatarUri);
                UserProfileUsername.Text = Common.DiscordUser.Username;
            }
            catch (Refit.ApiException ex)
            {
                Debug.WriteLine("API Exception:\n" + ex.ReasonPhrase);
            }

        }

        private async void RefreshProjects()
        {
            Projects.Clear();
            try
            {
                var projs = await Common.UwpCommApi.GetUserProjects(Common.DiscordUser.DiscordId);
                foreach (var project in projs)
                {
                    Projects.Add(project);
                }
            }
            catch (Refit.ApiException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // This means something went wrong with authentication,
                    // so attempt to log in again.
                    // TODO: Maybe this can be solved with refresh tokens?
                    Common.SignOut();
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
                    { "Proj", (p as Project).Id.ToString() },
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
                    await Common.UwpCommApi.DeleteProject(
                        new DeleteProjectRequest((p as Project).AppName));
                    RefreshProjects();
                    Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Dashboard: Delete project",
                        new Dictionary<string, string> {
                            { "Proj", (p as Project).Id.ToString() },
                        }
                    );
                }
                catch (Refit.ApiException ex)
                {
                    var error = await ex.GetContentAsAsync<Error>();
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
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Dashboard: View project",
                new Dictionary<string, string> {
                    { "Proj", (p as Project).Id.ToString() },
                }
            );
            NavigationManager.NavigateToViewProject(p);
        }

        private void RefreshContainer_RefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            RefreshProjects();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Dashboard: Navigated to",
                new System.Collections.Generic.Dictionary<string, string> {
                    { "From", e.SourcePageType.Name },
                    { "Parameters", e.Parameter?.ToString() }
                }
            );
        }
    }
}
