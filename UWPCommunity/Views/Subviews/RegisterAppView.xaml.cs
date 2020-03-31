using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using UWPCommLib.Api.UWPComm.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views.Subviews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterAppView : Page
    {
        public RegisterAppView()
        {
            this.InitializeComponent();
        }

        private async void SubmitButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(Common.DiscordToken);

            var project = new NewProjectRequest()
            {
                // Required
                AppName = NameBox.Text,
                Description = DescriptionBox.Text,
                IsPrivate = IsPrivateBox.IsChecked.Value,
                HeroImage = HeroUrlBox.Text,
                Category = (CategoryBox.SelectedValue == null) ? "Books and reference" : CategoryBox.SelectedValue.ToString(),
                Role = "Developer",

                // Optional
                DownloadLink = DownloadUrlBox.Text,
                GitHubLink = GithubUrlBox.Text,
                ExternalLink = ExternalUrlBox.Text,
            };

            await Common.UwpCommApi.PostProject(project);
        }

        private void CancelButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NavigationManager.PageFrame.GoBack();
        }
    }
}
