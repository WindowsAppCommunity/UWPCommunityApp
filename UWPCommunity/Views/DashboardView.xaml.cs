using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPCommLib.Api.UWPComm.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!Common.IsLoggedIn)
            {
                NavigationManager.RequestSignIn(typeof(DashboardView));
                return;
            }

            try
            {
                var projs = await Common.UwpCommApi.GetUserProjects(Common.DiscordUser.DiscordId);
                foreach (var project in projs)
                {
                    Projects.Add(project);
                }

                UserProfilePicture.ProfilePicture =
                    new Windows.UI.Xaml.Media.Imaging.BitmapImage(Common.DiscordUser.AvatarUri);
                UserProfileUsername.Text = Common.DiscordUser.Username;
            }
            catch (Refit.ApiException ex)
            {
                Debug.WriteLine("API Exception:\n" + ex.ReasonPhrase);
            }
            
            base.OnNavigatedTo(e);
        }

        private void RegisterAppButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate(typeof(Subviews.EditProjectView));
        }

        private void Project_EditRequested(object p)
        {
            NavigationManager.NavigateToEditProject(p);
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
                    await Common.UwpCommApi.DeleteProject((p as Project).AppName);
                }
                catch (Refit.ApiException ex)
                {
                    var error = ex.GetContentAs<Error>();
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
            NavigationManager.NavigateToViewProject(p);
        }
    }
}
