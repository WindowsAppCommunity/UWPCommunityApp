using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        public SettingsView()
        {
            this.InitializeComponent();

            ThemeBox.SelectedValue = SettingsManager.GetAppThemeName();
            UseDebugApiBox.IsChecked = SettingsManager.GetUseDebugApi();
            var cardSize = SettingsManager.GetProjectCardSize();
            ProjectCardWidth.Value = cardSize.X;
            ProjectCardHeight.Value = cardSize.Y;
            ShowLlamaBingoBox.IsChecked = SettingsManager.GetShowLlamaBingo();
            AppVersionRun.Text = App.GetVersion();

            SettingsManager.AppThemeChanged += SettingsManager_AppThemeChanged;
            SettingsManager.ProjectCardSizeChanged += SettingsManager_ProjectCardSizeChanged;
            SettingsManager.ShowLlamaBingoChanged += SettingsManager_ShowLlamaBingoChanged;
            SettingsManager.UseDebugApiChanged += SettingsManager_UseDebugApiChanged;
        }

        private void SettingsManager_UseDebugApiChanged(bool value)
        {
            UseDebugApiBox.IsChecked = value;
        }

        private void SettingsManager_ShowLlamaBingoChanged(bool value)
        {
            ShowLlamaBingoBox.IsChecked = value;
        }

        private void SettingsManager_ProjectCardSizeChanged(Point value)
        {
            ProjectCardWidth.Value = value.X;
            ProjectCardHeight.Value = value.Y;
        }

        private void SettingsManager_AppThemeChanged(ElementTheme value)
        {
            ThemeBox.SelectedValue = SettingsManager.GetAppThemeName();
        }

        private void ThemeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsManager.SetAppTheme(e.AddedItems[0].ToString());
        }

        private async void UseDebugApiBox_Changed(object sender, RoutedEventArgs e)
        {
            // Before switching, make sure that the selected source is
            // actually available
            bool newValue = UseDebugApiBox.IsChecked.Value;

            try
            {
                var client = new System.Net.Http.HttpClient();
                await client.GetAsync(newValue ? SettingsManager.DEBUG_API_URL : SettingsManager.PROD_API_URL);
                SettingsManager.SetUseDebugApi(newValue);
            }
            catch
            {
                ContentDialog dialogError = new ContentDialog
                {
                    Title = "Error",
                    Content = "A connection to the selected server could not be established",
                    PrimaryButtonText = "OK",
                    RequestedTheme = SettingsManager.GetAppTheme()
                };
                UseDebugApiBox.IsChecked = !newValue;
                await dialogError.ShowAsync();
            }
        }

        private void ProjectCardSize_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            SettingsManager.SetProjectCardSize(new Point(ProjectCardWidth.Value, ProjectCardHeight.Value));
        }

        private void ShowLlamaBingoBox_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetShowLlamaBingo(true);
        }

        private void ShowLlamaBingoBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetShowLlamaBingo(false);
        }

        private async void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Please confirm",
                Content = "Are you sure you want to reset your settings? This cannot be undone.",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "Cancel",
                RequestedTheme = SettingsManager.GetAppTheme()
            };
            ContentDialogResult result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    SettingsManager.LoadDefaults(true);
                    break;
            }
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Please confirm",
                Content = "Are you sure you want to reset the app? This cannot be undone.",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "Cancel",
                RequestedTheme = SettingsManager.GetAppTheme()
            };
            ContentDialogResult result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    SettingsManager.ResetApp();
                    break;
            }
        }

        private async void CrashButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Please confirm",
                Content = "This will crash the app. Are you sure you want to do this?",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "Cancel",
                RequestedTheme = SettingsManager.GetAppTheme()
            };
            ContentDialogResult result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    throw new Exception("User artificially threw an exception");
            }
        }
    }
}
