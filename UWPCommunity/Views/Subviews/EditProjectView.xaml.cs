using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using UWPCommLib.Api.UWPComm.Models;
using System;
using System.Linq;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views.Subviews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditProjectView : Page
    {
        private bool IsEditing { get; set; }
        private Project oldProject;
        private string oldAppName;

        public EditProjectView()
        {
            this.InitializeComponent();

            foreach (var category in Enum.GetValues(typeof(Project.ProjectCategory)).Cast<Project.ProjectCategory>())
            {
                CategoryBox.Items.Add(Project.GetCategoryTitle(category));
            }
            CategoryBox.SelectedIndex = 0;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var project = e.Parameter as Project;
            if (project != null)
            {
                IsEditing = true;
                MainPivotTitle.Text = "Edit " + project.AppName;
                oldProject = project;
                oldAppName = project.AppName;

                NameBox.Text = project.AppName;
                DescriptionBox.Text = project.Description;
                IsPrivateBox.IsChecked = project.IsPrivate;
                HeroUrlBox.Text = project.HeroImage;
                CategoryBox.SelectedValue = project.Category;
                DownloadUrlBox.Text = project.DownloadLink == null ? "" : project.DownloadLink;
                ExternalUrlBox.Text = project.ExternalLink == null ? "" : project.ExternalLink;
                GithubUrlBox.Text = project.GitHubLink == null ? "" : project.GitHubLink;
            }
        }

        private async void SubmitButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var project = new Project();// oldProject;
            project.AppName = NameBox.Text;
            project.Description = DescriptionBox.Text;
            project.IsPrivate = IsPrivateBox.IsChecked.Value;
            project.HeroImage = HeroUrlBox.Text;
            project.Category = (CategoryBox.SelectedValue == null) ? Project.GetCategoryTitle(0) : CategoryBox.SelectedValue.ToString();
            project.Role = String.IsNullOrEmpty(project.Role) ? "Developer" : project.Role;
            project.DownloadLink = DownloadUrlBox.Text;
            project.GitHubLink = GithubUrlBox.Text;
            project.ExternalLink = ExternalUrlBox.Text;
            project.IsAwaitingLaunchApproval = IsLaunchBox.IsChecked.Value;

            try
            {
                if (IsEditing)
                {
                    await Common.UwpCommApi.PutProject(oldAppName, project);
                }
                else
                {
                    await Common.UwpCommApi.PostProject(project);
                }
                NavigationManager.PageFrame.GoBack();
            }
            catch (Refit.ApiException ex)
            {
                var error = ex.GetContentAs<Error>();
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Failed to create project",
                    Content = error.Reason,
                    CloseButtonText = "Ok",
                    RequestedTheme = SettingsManager.GetAppTheme()
                };
                ContentDialogResult result = await dialog.ShowAsync();
            }
        }

        private void CancelButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NavigationManager.PageFrame.GoBack();
        }
    }
}
