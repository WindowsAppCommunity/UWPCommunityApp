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
                IconUrlBox.Text = project.AppIcon ?? "";
                CategoryBox.SelectedValue = project.Category;
                DownloadUrlBox.Text = project.DownloadLink ?? project.DownloadLink;
                ExternalUrlBox.Text = project.ExternalLink ?? project.ExternalLink;
                GithubUrlBox.Text = project.GitHubLink ?? project.GitHubLink;
            }
        }

        private async void SubmitButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var project = oldProject ?? new Project();
            project.AppName = NameBox.Text;
            project.Description = DescriptionBox.Text;
            project.IsPrivate = IsPrivateBox.IsChecked.Value;
            project.HeroImage = HeroUrlBox.Text;
            project.AppIcon = IconUrlBox.Text;
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
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Dashboard: App registration submitted",
                    new Dictionary<string, string> {
                        { "Project", project.ToString() }
                    }
                );
                NavigationManager.PageFrame.GoBack();
            }
            catch (Refit.ApiException ex)
            {
                var error = await ex.GetContentAsAsync<Error>();
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
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Dashboard: App registration canceled");
            NavigationManager.PageFrame.GoBack();
        }
    }
}
