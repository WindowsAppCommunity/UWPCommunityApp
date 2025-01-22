using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UwpCommunityBackend.Models;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using UWPCommunity.ViewModels;
using UwpCommunityBackend;
using System.Threading.Tasks;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectsView : Page
    {
        public double CardWidth { get; set; }
        public double CardHeight { get; set; }

        public ProjectsView()
        {
            InitializeComponent();

            var cardSize = SettingsManager.GetProjectCardSize();
            CardWidth = cardSize.X;
            CardHeight = cardSize.Y;

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

        private async void RefreshProjects()
        {
            try
            {
                var projs = (await Api.GetProjects()).OrderBy(x => x.AppName);
                foreach (var project in projs)
                {
                    ViewModel.Projects.Add(new ProjectViewModel(project));
                }
                ViewModel.AllProjects = ViewModel.Projects.ToList();
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
            Project project = null;
            if (sender is FrameworkElement frameworkElement)
            {
                if (frameworkElement.DataContext is Project proj)
                    project = proj;
                else if (frameworkElement.DataContext is ProjectViewModel projVm)
                    project = projVm.Project;
            }

            if (project != null)
            {
                await NavigationManager.OpenInBrowser(project.ExternalLink);
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Proj: External link badge clicked",
                    new Dictionary<string, string> {
                    { "Proj", project.Id.ToString() },
                    }
                );
            }
            else
            {
                // Tell the user that the link could not be opened
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Could not open external link",
                    CloseButtonText = "Ok",
                    RequestedTheme = SettingsManager.GetAppTheme()
                };
                await dialog.ShowAsync();
            }
        }

        private async void GitHubLinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Project project = null;
            if ((sender as Button)?.DataContext is Project _proj)
            {
                project = _proj;
            }
            else if ((sender as Button)?.DataContext is ProjectViewModel _projVM)
            {
                project = _projVM.Project;
            }

            if (project != null)
            {
                await NavigationManager.OpenInBrowser(project.GitHubLink);
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Proj: GitHub link badge clicked",
                    new Dictionary<string, string> {
                    { "Proj", project.Id.ToString() },
                    }
                );
            }
            else
            {
                // Tell the user that the link could not be opened
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Could not open GitHub link",
                    CloseButtonText = "Ok",
                    RequestedTheme = SettingsManager.GetAppTheme()
                };
                ContentDialogResult result = await dialog.ShowAsync();
            }
        }

        private async void DownloadLinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Project project = null;
            if ((sender as Button)?.DataContext is Project _proj)
			{
                project = _proj;
			}
            else if ((sender as Button)?.DataContext is ProjectViewModel _projVM)
            {
                project = _projVM.Project;
            }

            if (project != null)
			{
                await NavigationManager.OpenInBrowser(project.DownloadLink);
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Proj: Download link badge clicked",
                    new Dictionary<string, string> {
                    { "Proj", project.Id.ToString() },
                    }
                );
            }
            else
			{
                // Tell the user that the link could not be opened
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Could not open download link",
                    CloseButtonText = "Ok",
                    RequestedTheme = SettingsManager.GetAppTheme()
                };
                ContentDialogResult result = await dialog.ShowAsync();
            }
        }

        private void Project_ViewRequested(object p)
        {
            ViewProject((p as ProjectViewModel).Project);
        }

        private void ViewProject(Project item)
        {
            //ProjectsGridView.PrepareConnectedAnimation("projectView", item, "HeroImageStartCtl");
            NavigationManager.NavigateToViewProject(item);
        }

        private void ProjectsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewProject((e.ClickedItem as ProjectViewModel).Project);
        }

        private void RefreshContainer_RefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            RefreshProjects();
        }

        private async void SortOption_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Sort(((MenuFlyoutItem)sender).Text);
        }

        private async void CategoryItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CategoryButton.IsChecked = true;
            SearchBox.Text = "";
            // TODO: Check for null here
            await FilterByCategory(((MenuFlyoutItem)sender).Text);
            Bindings.Update();
        }

        private async void CategoryButton_IsCheckedChanged(Microsoft.UI.Xaml.Controls.ToggleSplitButton sender, Microsoft.UI.Xaml.Controls.ToggleSplitButtonIsCheckedChangedEventArgs args)
        {
            SearchBox.Text = "";
            if (sender.IsChecked)
            {
                // Filter is enabled
                await FilterByCategory();
            }
            else
            {
                // Filter is disabled
                ViewModel.Projects = new ObservableCollection<ProjectViewModel>(ViewModel.AllProjects);
                // Must call Sort() here because FilterByCategory() won't
                // do it for us in this branch
                await Sort();
            }
            Bindings.Update();
        }

        private async Task SearchByName(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ViewModel.Projects = new ObservableCollection<ProjectViewModel>(ViewModel.AllProjects);
                Bindings.Update();
                return;
            }
            var results = ViewModel.AllProjects.Where(x =>
                x.Project.AppName.ToLower().Contains(query.ToLower()) ||
                x.Project.Description.ToLower().Contains(query.ToLower()));

            if (CategoryButton.IsChecked)
                FilterByCategory(collection: results);
            else
                await Sort(collection: results);

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Projects: Search",
                new Dictionary<string, string> {
                    { "Query", query },
                }
            );
        }

        private async Task FilterByCategory(string category = null, IEnumerable<ProjectViewModel> collection = null)
        {
            collection ??= ViewModel.AllProjects;

            if (category == null)
            {
                // Get first selected item
                var option = (RadioMenuFlyoutItem)CategoryFlyout.Items.FirstOrDefault(i => (i as RadioMenuFlyoutItem).IsChecked);
                category = (option == default(RadioMenuFlyoutItem))
                    ? ((RadioMenuFlyoutItem)CategoryFlyout.Items[0]).Text : option.Text;
            }

            await Sort(collection: collection.Where(x => x.Project.Category.Equals(category)));

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Projects: Category filter",
                new Dictionary<string, string> {
                    { "Category", category },
                }
            );
        }

        private async Task Sort(string sortMode = null, IEnumerable<ProjectViewModel> collection = null)
        {
            if (sortMode == null)
            {
                var sortOption = SortFlyout.Items
                    .OfType<RadioMenuFlyoutItem>()
                    .FirstOrDefault(i => i.IsChecked);
                sortMode = sortOption?.Text;
            }
            
            var sorted = await Task.Run(() => {
                collection ??= ViewModel.AllProjects;

                IOrderedEnumerable<ProjectViewModel> sorted = sortMode switch
                {
                    "Alphabetical (A-Z)" => collection.OrderBy(x => x.Project.AppName),
                    "Alphabetical (Z-A)" => collection.OrderByDescending(x => x.Project.AppName),
                    "Date Created (Latest-Oldest)" => collection.OrderByDescending(x => x.Project.CreatedAt),
                    "Date Created (Oldest-Latest)" => collection.OrderBy(x => x.Project.CreatedAt),
                    "Last Modified (Latest-Oldest)" => collection.OrderByDescending(x => x.Project.UpdatedAt),
                    "Last Modified (Oldest-Latest)" => collection.OrderBy(x => x.Project.UpdatedAt),
                    "Launch Year (Latest-Oldest)" => collection.OrderByDescending(x => x.Project.GetLastLaunchYear() ?? 0),
                    "Launch Year (Oldest-Latest)" => collection.OrderBy(x => x.Project.GetLastLaunchYear() ?? 0),
                    _ => collection.OrderBy(x => x.Project.AppName),
                };
                return sorted;
            });

            ViewModel.Projects.Clear();
            foreach (var project in sorted)
                ViewModel.Projects.Add(project);

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Projects: Sort",
                new Dictionary<string, string> {
                    { "Mode", sortMode },
                }
            );
        }

        private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await SearchByName(args.QueryText);
        }
    }
}
