using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace UWPCommunity
{
    public static class SettingsManager
    {
        // Load the app's settings
        private static Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private static Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        public static ElementTheme GetAppTheme()
        {
            return ThemeFromName(localSettings.Values["AppTheme"] as string);
        }
        public static string GetAppThemeName()
        {
            var theme = localSettings.Values["AppTheme"] as string;
            if (String.IsNullOrEmpty(theme))
            {
                var defaultTheme = "Default";
                SetAppTheme(defaultTheme);
                return defaultTheme;
            }
            else
            {
                return theme;
            }
        }
        public static void SetAppTheme(ElementTheme theme)
        {
            localSettings.Values["AppTheme"] = theme.ToString("g");
            ApplyAppTheme(theme);
            AppThemeChanged?.Invoke(theme);
        }
        public static void SetAppTheme(string themeString)
        {
            SetAppTheme(ThemeFromName(themeString));
        }
        private static ElementTheme ThemeFromName(string themeName)
        {
            switch (themeName)
            {
                case "Light":
                    return ElementTheme.Light;

                case "Dark":
                    return ElementTheme.Dark;

                default:
                    return ElementTheme.Default;
            }
        }
        public static void ApplyAppTheme(ElementTheme theme)
        {
            // Set theme for window root.
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = theme;
            }
        }
        public delegate void AppThemeChangedHandler(ElementTheme value);
        public static event AppThemeChangedHandler AppThemeChanged;

        public static bool GetUseDebugApi()
        {
            try
            {
                return Boolean.Parse(localSettings.Values["UseDebugApi"] as string);
            }
            catch
            {
                SetUseDebugApi(false);
                return false;
            }
        }
        public static void SetUseDebugApi(bool value)
        {
            localSettings.Values["UseDebugApi"] = value.ToString();
            ApplyUseDebugApi(value);
            UseDebugApiChanged?.Invoke(value);
        }
        public static void ApplyUseDebugApi(bool value)
        {
            Common.UwpCommApiHostUrl =
                value ? "http://localhost:5000" : "https://uwpcommunity-site-backend.herokuapp.com";
            Common.UwpCommApi = Refit.RestService.For<UWPCommLib.Api.UWPComm.IUwpCommApi>(
                Common.UwpCommApiHostUrl
            );
        }
        public delegate void UseDebugApiChangedHandler(bool value);
        public static event UseDebugApiChangedHandler UseDebugApiChanged;

        public static Point GetProjectCardSize()
        {
            try
            {
                return (Point)localSettings.Values["ProjectCardSize"];
            }
            catch
            {
                var defaultRect = new Point(300, 300);
                SetProjectCardSize(defaultRect);
                return defaultRect;
            }
        }
        public static void SetProjectCardSize(Point value)
        {
            localSettings.Values["ProjectCardSize"] = value;
            ProjectCardSizeChanged?.Invoke(value);
        }
        public delegate void ProjectCardSizeChangedHandler(Point value);
        public static event ProjectCardSizeChangedHandler ProjectCardSizeChanged;

        public static bool GetShowLlamaBingo()
        {
            try
            {
                return (bool)localSettings.Values["ShowLlamaBingo"];
            }
            catch
            {
                SetShowLlamaBingo(true);
                return true;
            }
        }
        public static void SetShowLlamaBingo(bool value)
        {
            localSettings.Values["ShowLlamaBingo"] = value;
            ShowLlamaBingoChanged?.Invoke(value);
        }
        public delegate void ShowLlamaBingoChangedHandler(bool value);
        public static event ShowLlamaBingoChangedHandler ShowLlamaBingoChanged;
    }
}
