using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPCommLib.Api.Yoshi.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views.Dialogs
{
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
        }
        public AppMessageDialog(AppMessage message)
        {
            this.InitializeComponent();
            Message = message;
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
