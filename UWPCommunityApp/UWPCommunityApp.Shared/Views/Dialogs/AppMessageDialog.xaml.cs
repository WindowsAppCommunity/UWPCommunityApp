using System;
using UWPCommLib.Api.Yoshi.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunityApp.Views.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppMessageDialog : ContentDialog
    {
        public AppMessage Message { get; set; }
        public string DateString {
            get {
                var date = new DateTime(1970, 1, 1).AddSeconds(Message.Timestamp).ToLocalTime();
                return date.ToShortDateString() + " " + date.ToShortTimeString();
            }
        }

        public AppMessageDialog()
        {
            this.InitializeComponent();
            RequestedTheme = SettingsManager.GetAppTheme();
        }
        public AppMessageDialog(AppMessage message)
        {
            this.InitializeComponent();
            Message = message;
            RequestedTheme = SettingsManager.GetAppTheme();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Do nothing, just carry on to the app
        }

        private void PreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            NavigationManager.NavigateToSettings(SettingsPages.AppMessages);
        }
    }
}
