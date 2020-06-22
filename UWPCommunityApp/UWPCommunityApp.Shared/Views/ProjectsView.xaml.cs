using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UWPCommLib.Api.UWPComm.Models;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using RefreshContainer = Windows.UI.Xaml.Controls.RefreshContainer;
using RefreshRequestedEventArgs = Windows.UI.Xaml.Controls.RefreshRequestedEventArgs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunityApp.Views
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
#if WINDOWS_UWP
            foreach (var category in Enum.GetValues(typeof(Project.ProjectCategory)).Cast<Project.ProjectCategory>())
            {
                var menuItem = new RadioMenuFlyoutItem()
                {
                    Text = Project.GetCategoryTitle(category)
                };
                menuItem.Click += CategoryItem_Click;
                CategoryFlyout.Items.Add(menuItem);
            }
#else
            CategoryButton.IsEnabled = false;
#endif
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
            try
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
            catch (HttpRequestException ex)
            {
                NavigationManager.NavigateToReconnect(ex);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Projects: Navigated to",
                new Dictionary<string, string> {
                    { "From", e.SourcePageType.Name },
                    { "Parameters", e.Parameter?.ToString() }
                }
            );
        }

        private async void ExternalLinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var project = (sender as Button)?.DataContext as Project;
            await NavigationManager.OpenInBrowser(project.ExternalLink);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Proj: External link badge clicked",
                new Dictionary<string, string> {
                    { "Proj", project.Id.ToString() },
                }
            );
        }

        private async void GitHubLinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var project = (sender as Button)?.DataContext as Project;
            await NavigationManager.OpenInBrowser(project.GitHubLink);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Proj: GitHub link badge clicked",
                new Dictionary<string, string> {
                    { "Proj", project.Id.ToString() },
                }
            );
        }

        private async void DownloadLinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var project = (sender as Button)?.DataContext as Project;
            await NavigationManager.OpenInBrowser(project.DownloadLink);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Proj: Download link badge clicked",
                new Dictionary<string, string> {
                    { "Proj", project.Id.ToString() },
                }
            );
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

        private void RefreshContainer_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            RefreshProjects();
        }

        private void SortOption_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Sort("Alphabetical (A-Z)");//(sender as RadioMenuFlyoutItem).Text);
        }

        private void CategoryItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CategoryButton.IsChecked = true;
            SearchBox.Text = "";
            //var option = sender as RadioMenuFlyoutItem;
            FilterByCategory(null);//option.Text);
            Bindings.Update();
        }

        private void CategoryButton_IsCheckedChanged(Windows.UI.Xaml.Controls.ToggleSplitButton sender, Windows.UI.Xaml.Controls.ToggleSplitButtonIsCheckedChangedEventArgs args)
        {
            SearchBox.Text = "";
            if (sender.IsChecked)
            {
                // Filter is enabled
                FilterByCategory();
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

        private void SearchByName(string query)
        {
            if (String.IsNullOrWhiteSpace(query))
            {
                Projects = new ObservableCollection<Project>(AllProjects);
                Bindings.Update();
                return;
            }
            var results = AllProjects.Where(x =>
                x.AppName.ToLower().Contains(query.ToLower()) ||
                x.Description.ToLower().Contains(query.ToLower()));

            if (CategoryButton.IsChecked)
                FilterByCategory(collection: results);
            else
                Sort(collection: results);

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Projects: Search",
                new Dictionary<string, string> {
                    { "Query", query },
                }
            );
        }

        private void FilterByCategory(string category = null, IEnumerable<Project> collection = null)
        {
            if (collection == null)
                collection = AllProjects;

            if (category == null)
            {
#if WINDOWS_UWP
                // Get first selected item
                var option = (RadioMenuFlyoutItem)CategoryFlyout.Items.FirstOrDefault(i => (i as RadioMenuFlyoutItem).IsChecked);
                category = (option == default(RadioMenuFlyoutItem))
                    ? ((RadioMenuFlyoutItem)CategoryFlyout.Items[0]).Text : option.Text;
                Sort(collection: collection.Where(x => x.Category.Equals(category)));
#else
                // TODO: Uno doesn't have the RadioMenuFlyoutItem or ToggleSplitButton,
                // so just hardcode this until there's a better fix.
                Sort(collection: collection);
#endif
            }

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Projects: Category filter",
                new Dictionary<string, string> {
                    { "Category", category },
                }
            );
        }

        private void Sort(string mode = null, IEnumerable<Project> collection = null)
        {
            if (collection == null)
                collection = AllProjects;
            if (mode == null)
            {
                //var sortOption = (RadioMenuFlyoutItem)SortFlyout.Items.First(i => (i as RadioMenuFlyoutItem).IsChecked);
                //mode = sortOption.Text;
                mode = "Alphabetical (A-Z)";
            }

            IOrderedEnumerable<Project> sorted;
            switch (mode)
            {
                case "Alphabetical (A-Z)":
                    sorted = collection.OrderBy(x => x.AppName);
                    break;
                case "Alphabetical (Z-A)":
                    sorted = collection.OrderByDescending(x => x.AppName);
                    break;

                case "Date Created (Latest-Oldest)":
                    sorted = collection.OrderByDescending(x => DateTime.Parse(x.CreatedAt));
                    break;
                case "Date Created (Oldest-Latest)":
                    sorted = collection.OrderBy(x => DateTime.Parse(x.CreatedAt));
                    break;

                case "Last Modified (Latest-Oldest)":
                    sorted = collection.OrderByDescending(x => DateTime.Parse(x.UpdatedAt));
                    break;
                case "Last Modified (Oldest-Latest)":
                    sorted = collection.OrderBy(x => DateTime.Parse(x.UpdatedAt));
                    break;

                case "Launch Year (Latest-Oldest)":
                    sorted = collection.OrderByDescending(x => x.LaunchYear);
                    break;
                case "Launch Year (Oldest-Latest)":
                    sorted = collection.OrderBy(x => x.LaunchYear);
                    break;

                default:
                    sorted = collection.OrderBy(x => x.AppName);
                    break;
            }
            Projects = new ObservableCollection<Project>(sorted);
            Bindings.Update();

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Projects: Sort",
                new Dictionary<string, string> {
                    { "Mode", mode },
                }
            );
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            SearchByName(args.QueryText);
        }
    }
}
