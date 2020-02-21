using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;
using UWPCommunity.Views;

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
            Navigate(typeof(HomeView));
        }

        public static void RequestSignIn(Type returnToPage)
        {
            if (!Common.IsLoggedIn)
                PageFrame.Navigate(typeof(LoginView), returnToPage);
            else
                PageFrame.Navigate(returnToPage);
        }

        public static async Task<bool> OpenDiscordInvite(string inviteCode)
        {
            var quarrelLaunchUri = new Uri("quarrel://invite/" + inviteCode);
            var launchUri = new Uri("https://discord.gg/" + inviteCode);
            switch (await Launcher.QueryUriSupportAsync(quarrelLaunchUri, LaunchQuerySupportType.Uri)) {
                case LaunchQuerySupportStatus.Available:
                    return await Launcher.LaunchUriAsync(quarrelLaunchUri);

                default:
                    return await Launcher.LaunchUriAsync(launchUri);
            }
        }

        public static void Navigate(Type destinationPage)
        {
            PageFrame.Navigate(destinationPage);
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
