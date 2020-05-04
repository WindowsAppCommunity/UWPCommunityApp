using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UWPCommLib.Api.UWPComm.Models;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectsView : Page
    {
        private List<Project> AllProjects { get; set; }
        public ObservableCollection<Project> Projects { get; set; } = new ObservableCollection<Project>();

        public ProjectsView()
        {
            InitializeComponent();
            Loaded += ProjectsView_Loaded;

            foreach (var category in Enum.GetValues(typeof(Project.ProjectCategory)).Cast<Project.ProjectCategory>())
            {
                var menuItem = new RadioMenuFlyoutItem()
                {
                    Text = Project.GetCategoryTitle(category)
                };
                menuItem.Click += CategoryItem_Click;
                CategoryFlyout.Items.Add(menuItem);
            }
            RefreshProjects();
        }

        private void ProjectsView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var cardSize = SettingsManager.GetProjectCardSize();
            ProjectsGridView.DesiredWidth = cardSize.X;
            ProjectsGridView.ItemHeight = cardSize.Y;
        }

        private async void RefreshProjects()
        {
            var projs = (await Common.UwpCommApi.GetProjects()).OrderBy(x => x.AppName);
            Projects = new ObservableCollection<Project>(projs);
            AllProjects = Projects.ToList();
            if (ProjectsGridView.Items.Count != Projects.Count)
            {
                Bindings.Update();
            }
            LoadingIndicator.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void ExternalLinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var project = (sender as Button)?.DataContext as Project;
            await NavigationManager.OpenInBrowser(project.ExternalLink);
        }

        private async void GitHubLinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var project = (sender as Button)?.DataContext as Project;
            await NavigationManager.OpenInBrowser(project.GitHubLink);
        }

        private async void DownloadLinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var project = (sender as Button)?.DataContext as Project;
            await NavigationManager.OpenInBrowser(project.DownloadLink);
        }

        private void Project_ViewRequested(object p)
        {
            ViewProject(p as Project);
        }

        private void ViewProject(Project item)
        {
            ProjectsGridView.PrepareConnectedAnimation("projectView", item, "HeroImageStartCtl");
            NavigationManager.NavigateToViewProject(item);
        }

        private void ProjectsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewProject(e.ClickedItem as Project);
        }

        private void RefreshContainer_RefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            RefreshProjects();
        }

        private void SortOption_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Sort((sender as RadioMenuFlyoutItem).Text);
        }

        private void CategoryItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CategoryButton.IsChecked = true;
            var option = sender as RadioMenuFlyoutItem;
            FilterByCategory(option.Text);
            Bindings.Update();
        }

        private void CategoryButton_IsCheckedChanged(Microsoft.UI.Xaml.Controls.ToggleSplitButton sender, Microsoft.UI.Xaml.Controls.ToggleSplitButtonIsCheckedChangedEventArgs args)
        {
            if (sender.IsChecked)
            {
                // Filter is enabled
                // Get first selected item
                var option = (RadioMenuFlyoutItem)CategoryFlyout.Items.First(i => (i as RadioMenuFlyoutItem).IsChecked);
                FilterByCategory(option.Text);
            }
            else
            {
                // Filter is disabled
                Projects = new ObservableCollection<Project>(AllProjects);
                // Must call Sort() here because FilterByCategory() won't
                // do it for us in this branch
                Sort();
            }
            Bindings.Update();
        }

        private void FilterByCategory(string category)
        {
            Projects = new ObservableCollection<Project>(
                AllProjects.Where(x => x.Category.Equals(category))
            );
            Sort();
        }

        private void Sort(string mode = null)
        {
            if (mode == null)
            {
                var sortOption = (RadioMenuFlyoutItem)SortFlyout.Items.First(i => (i as RadioMenuFlyoutItem).IsChecked);
                mode = sortOption.Text;
            }

            IOrderedEnumerable<Project> sorted;
            switch (mode)
            {
                case "Alphabetical (A-Z)":
                    sorted = Projects.OrderBy(x => x.AppName);
                    break;
                case "Alphabetical (Z-A)":
                    sorted = Projects.OrderByDescending(x => x.AppName);
                    break;

                case "Date Created (Latest-Oldest)":
                    sorted = Projects.OrderByDescending(x => DateTime.Parse(x.CreatedAt));
                    break;
                case "Date Created (Oldest-Latest)":
                    sorted = Projects.OrderBy(x => DateTime.Parse(x.CreatedAt));
                    break;

                case "Last Modified (Latest-Oldest)":
                    sorted = Projects.OrderByDescending(x => DateTime.Parse(x.UpdatedAt));
                    break;
                case "Last Modified (Oldest-Latest)":
                    sorted = Projects.OrderBy(x => DateTime.Parse(x.UpdatedAt));
                    break;

                case "Launch Year (Latest-Oldest)":
                    sorted = Projects.OrderByDescending(x => x.LaunchYear);
                    break;
                case "Launch Year (Oldest-Latest)":
                    sorted = Projects.OrderBy(x => x.LaunchYear);
                    break;

                default:
                    sorted = Projects.OrderBy(x => x.AppName);
                    break;
            }
            Projects = new ObservableCollection<Project>(sorted);
            Bindings.Update();
        }
    }
}
