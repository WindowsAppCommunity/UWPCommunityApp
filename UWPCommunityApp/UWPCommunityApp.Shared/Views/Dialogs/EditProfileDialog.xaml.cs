using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunityApp.Views.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditProfileDialog : ContentDialog
    {
        public EditProfileDialog()
        {
            this.InitializeComponent();
            Loaded += EditProfileDialog_Loaded;
            RequestedTheme = SettingsManager.GetAppTheme();
        }

        private async void EditProfileDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var user = await Common.GetCurrentUser();
#if WINDOWS_UWP
            LoadingBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
#elif DROID
            LoadingBar.Visibility = Android.Views.ViewStates.Gone;
#endif

            NameBox.Text = user.Name;
            EmailBox.Text = user.Email ?? "";
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // User wants to apply the changes
            var changes = new Dictionary<string, string>() {
                { "name", NameBox.Text }
            };
            if (!String.IsNullOrWhiteSpace(EmailBox.Text))
            {
                changes.Add("email", EmailBox.Text);
            }
            await Common.UwpCommApi.SetUser(changes);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // User wants to cancel
        }
    }
}
