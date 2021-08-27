using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;
using UWPCommunity.Views;
using Windows.UI.Xaml;
using Flurl;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

namespace UWPCommunity
{
    public static class NavigationManager
    {
        public static Frame PageFrame { get; set; }

        public static void NavigateToDashboard()
        {
            RequestSignIn(typeof(DashboardView));
        }

        public static void NavigateToHome()
        {
            Navigate(typeof(HomeView));
        }

        public static void NavigateToSettings()
        {
            Navigate(typeof(SettingsView));
        }
        public static void NavigateToSettings(SettingsPages page)
        {
            Navigate(typeof(SettingsView), page);
        }

        public static async void RequestSignIn(Type returnToPage)
        {
            if (!UserManager.IsLoggedIn)
            {
                var privacyPolicyResult = await new Views.Dialogs.ConfirmPrivacyPolicyDialog().ShowAsync();
                if (privacyPolicyResult != ContentDialogResult.Primary)
                    return;

                PageFrame.Navigate(typeof(LoginView), returnToPage);
            }
            else
                PageFrame.Navigate(returnToPage);
        }

        public async static Task<bool> OpenInBrowser(Uri uri)
        {
            return await Launcher.LaunchUriAsync(uri);
        }
        public async static Task<bool> OpenInBrowser(string url)
        {
            // Wrap in a try-catch block in order to prevent the
            // app from crashing from invalid links.
            // (specifically from project badges)
            try
            {
                return await OpenInBrowser(new Uri(url));
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> OpenDiscordInvite(string inviteCode)
        {
            var quarrelLaunchUri = new Uri("quarrel://invite/" + inviteCode);
            var launchUri = new Uri("https://discord.gg/" + inviteCode);
            switch (await Launcher.QueryUriSupportAsync(quarrelLaunchUri, LaunchQuerySupportType.Uri)) {
                case LaunchQuerySupportStatus.Available:
                    return await Launcher.LaunchUriAsync(quarrelLaunchUri);

                default:
                    return await OpenInBrowser(launchUri);
            }
        }

        public static void Navigate(Type destinationPage)
        {
            PageFrame.Navigate(destinationPage);
        }
        public static void Navigate(Type destinationPage, object parameter)
        {
            PageFrame.Navigate(destinationPage, parameter);
        }

        public static void NavigateToEditProject(object project)
        {
            PageFrame.Navigate(typeof(Views.Subviews.EditProjectView), project);
        }

        public static void NavigateToViewProject(object project)
        {
            PageFrame.Navigate(typeof(Views.Subviews.ProjectDetailsView), project);
        }

        public static void NavigateToReconnect(System.Net.Http.HttpRequestException ex)
        {
            (Window.Current.Content as Frame).Navigate(
                typeof(Views.Subviews.NoInternetPage), ex
            );
        }

        public static void RemovePreviousFromBackStack()
        {
            PageFrame.BackStack.RemoveAt(PageFrame.BackStack.Count - 1);
        }

        public static Tuple<Type, object> ParseProtocol(Url ptcl)
        {
            Type destination = typeof(HomeView);
            var defaultResult = new Tuple<Type, object>(destination, null);

            if (ptcl == null || string.IsNullOrWhiteSpace(ptcl.Path))
                return defaultResult;

            try
            {
                string scheme = ptcl.Path.Split(":")[0];
                string path;
                switch (scheme)
                {
                    case "http":
                        path = ptcl.ToString().Remove(0, 23);
                        break;

                    case "https":
                        path = ptcl.ToString().Remove(0, 24);
                        break;

                    case "uwpcommunity":
                        path = ptcl.ToString().Remove(0, scheme.Length + 3);
                        break;

                    default:
                        // Unrecognized protocol
                        return defaultResult;
                }
                if (path.StartsWith("/"))
                    path = path.Remove(0, 1);
                var queryParams = System.Web.HttpUtility.ParseQueryString(ptcl.Query.Replace("\r", String.Empty).Replace("\n", String.Empty));

                string rootPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries)[0];
                switch (rootPath)
                {
                    case "nointernet":
                        return new Tuple<Type, object>(typeof(Views.Subviews.NoInternetPage), queryParams);

                    default:
                        PageInfo pageInfo = MainPage.Pages.Find(p => p.Path == rootPath);
                        destination = pageInfo != null ? pageInfo.PageType : typeof(HomeView);
                        return new Tuple<Type, object>(destination, queryParams);
                }
            }
            catch
            {
                return defaultResult;
            }
        }
        public static Tuple<Type, object> ParseProtocol(string url)
        {
            return ParseProtocol(String.IsNullOrWhiteSpace(url) ? null : new Url(url));
        }
    }

    public class PageInfo
    {
        public PageInfo() {}
        
        public PageInfo(string title, string subhead, IconElement icon)
        {
            Title = title;
            Subhead = subhead;
            Icon = icon;
        }

        public PageInfo(NavigationViewItem navItem)
        {
            Title = navItem.Content?.ToString() ?? string.Empty;
            Icon = navItem.Icon ?? new SymbolIcon(Symbol.Document);
            Visibility = navItem.Visibility;

            var tooltip = ToolTipService.GetToolTip(navItem);
            Tooltip = tooltip?.ToString() ?? string.Empty;
        }

        public string Title { get; set; }
        public string Subhead { get; set; }
        public IconElement Icon { get; set; }
        public Type PageType { get; set; }
        public string Path { get; set; }
        public string Tooltip { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Visible;
        public bool RequiresAuth { get; set; } = false;

        // Derived properties
        public NavigationViewItem NavViewItem {
            get
            {
                var item = new NavigationViewItem()
                {
                    Tag = this,
                    Icon = Icon,
                    Content = Title,
                    Visibility = Visibility
                };
                ToolTipService.SetToolTip(item, new ToolTip() { Content = Tooltip });

                return item;
            }
        }
        public string Protocol => "uwpcommunity://" + Path;
        public Uri IconAsset => new Uri("ms-appx:///Assets/Icons/" + Path + ".png");
    }

    public enum SettingsPages
    {
        General,
        AppMessages,
        About,
        Debug
    }
}
