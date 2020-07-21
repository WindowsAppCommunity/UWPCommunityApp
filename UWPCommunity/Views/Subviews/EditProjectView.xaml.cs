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
        private static bool IsEditing { get; set; }
        private Project Project { get; set; }
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

        public static string GetPageHeader(string appName)
        {
            return IsEditing ? "Editing " + appName : "Register an app";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var project = e.Parameter as Project;
            if (project == null)
            {
                Project = new Project();
                IsEditing = false;
            }
            else
            {
                oldAppName = project.AppName;
                Project = project;
                IsEditing = true;
            }
            Bindings.Update();
        }

        private async void SubmitButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                if (IsEditing)
                {
                    await Common.UwpCommApi.PutProject(oldAppName, Project);
                }
                else
                {
                    await Common.UwpCommApi.PostProject(Project);
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
        
        private async void SaveDraftButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await SettingsManager.SaveProjectDraft(Project, !IsEditing);
        }

        private async void LoadDraftButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Project draft = await SettingsManager.LoadProjectDraft(Project.Id, !IsEditing);
            if (draft != null)
            {
                Project = draft;
                Bindings.Update();
            }
            else
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Failed to load draft",
                    Content = "No drafts have been saved for this project",
                    CloseButtonText = "Ok",
                    RequestedTheme = SettingsManager.GetAppTheme()
                };
                ContentDialogResult result = await dialog.ShowAsync();
                return;
            }
        }

        private void CancelButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NavigationManager.PageFrame.GoBack();
        }
    }
}
