using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views.Dialogs
{
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
            LoadingBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

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
