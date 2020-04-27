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
        }

        private void ThemeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsManager.SetAppTheme(e.AddedItems[0].ToString());
        }

        private void UseDebugApiBox_Changed(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetUseDebugApi(UseDebugApiBox.IsChecked.Value);
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
    }
}
