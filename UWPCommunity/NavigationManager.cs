using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;
using UWPCommunity.Views;
using Windows.UI.Xaml.Media.Animation;

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

        public static async void RequestSignIn(Type returnToPage)
        {
            if (!Common.IsLoggedIn)
            {
                var privacyPolicyResult = await (new Views.Dialogs.ConfirmPrivacyPolicyDialog().ShowAsync());
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

        public static void RemovePreviousFromBackStack()
        {
            PageFrame.BackStack.RemoveAt(PageFrame.BackStack.Count - 1);
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
            Title = (navItem.Content == null) ? "" : navItem.Content.ToString();
            Icon = (navItem.Icon == null) ? new SymbolIcon(Symbol.Document) : navItem.Icon;
            Visibility = navItem.Visibility;

            var tooltip = ToolTipService.GetToolTip(navItem);
            Tooltip = (tooltip == null) ? "" : tooltip.ToString();
        }

        public string Title { get; set; }
        public string Subhead { get; set; }
        public IconElement Icon { get; set; }
        public Type PageType { get; set; }
        public string Tooltip { get; set; }
        public Windows.UI.Xaml.Visibility Visibility { get; set; } = Windows.UI.Xaml.Visibility.Visible;

        public NavigationViewItem NavViewItem {
            get {
                var item = new NavigationViewItem()
                {
                    Icon = Icon,
                    Content = Title,
                    Visibility = Visibility
                };
                ToolTipService.SetToolTip(item, new ToolTip() { Content = Tooltip });

                return item;
            }
        }
    }
}
