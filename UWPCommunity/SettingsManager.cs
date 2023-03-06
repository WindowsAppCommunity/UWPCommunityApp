using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Threading.Tasks;
using UwpCommunityBackend.Models;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace UWPCommunity
{
    public static class SettingsManager
    {
        // Load the app's settings
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        public static async Task SaveProjectDraft(Project proj, bool isNewApp = false)
        {
            var folder = await localFolder.CreateFolderAsync("ProjectDrafts", CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync(
                (isNewApp ? "newapp" : proj.Id.ToString()) + ".json",
                CreationCollisionOption.ReplaceExisting
            );
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(proj);
            await FileIO.WriteTextAsync(file, json);
        }
        public static async Task<Project> LoadProjectDraft(int id, bool isNewApp = false)
        {
            try
            {
                var folder = await localFolder.CreateFolderAsync("ProjectDrafts", CreationCollisionOption.OpenIfExists);
                var file = await folder.GetFileAsync(
                    (isNewApp ? "newapp" : id.ToString()) + ".json"
                );
                var proj = Newtonsoft.Json.JsonConvert.DeserializeObject<Project>(
                    await FileIO.ReadTextAsync(file)
                );
                return proj;
            }
            catch (System.IO.FileNotFoundException ex)
            {
                return null;
            }
        }

        public static void LoadDefaults(bool overrideCurr = true)
        {
            if (!localSettings.Values.ContainsKey("AppTheme") || overrideCurr)
                SetAppTheme("Default");
            if (!localSettings.Values.ContainsKey("UseDebugApi") || overrideCurr)
                SetUseDebugApi(false);
            if (!localSettings.Values.ContainsKey("ProjectCardSize") || overrideCurr)
                SetProjectCardSize(new Point(400, 300));
            if (!localSettings.Values.ContainsKey("ShowLlamaBingo") || overrideCurr)
                SetShowLlamaBingo(true);
            if (!localSettings.Values.ContainsKey("SavedLlamaBingo") || overrideCurr)
                SetSavedLlamaBingo(null);
            if (!localSettings.Values.ContainsKey("ShowLiveTile") || overrideCurr)
                SetShowLiveTile(true);
            if (!localSettings.Values.ContainsKey("ExtendIntoTitleBar") || overrideCurr)
                SetExtendIntoTitleBar(true);
            AppMessageSettings.LoadDefaults(overrideCurr);
        }

        public static async void ResetApp()
        {
            // TODO: This currently doesn't work, because the app is still running.
            await ApplicationData.Current.ClearAsync();
        }

        public static ElementTheme GetAppTheme()
        {
            return ThemeFromName(GetAppThemeName());
        }
        public static string GetAppThemeName()
        {
            if (localSettings.Values.TryGetValue("AppTheme", out object value))
            {
                return value.ToString();
            }
            else
            {
                var defaultTheme = "Default";
                SetAppTheme(defaultTheme);
                return defaultTheme;
            }
        }
        public static void SetAppTheme(ElementTheme theme)
        {
            localSettings.Values["AppTheme"] = theme.ToString("g");
            ApplyAppTheme(theme);
            AppThemeChanged?.Invoke(theme);
            SettingsChanged?.Invoke("AppTheme", theme);
        }
        public static void SetAppTheme(string themeString)
        {
            SetAppTheme(ThemeFromName(themeString));
        }
        private static ElementTheme ThemeFromName(string themeName)
        {
            return (ElementTheme)Enum.Parse(typeof(ElementTheme), themeName, true);
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

        public const string DEBUG_API_URL = "http://localhost:5000";
        public const string PROD_API_URL = "https://uwpcommunity-site-backend.herokuapp.com";
        public static bool GetUseDebugApi()
        {
            if (localSettings.Values.TryGetValue("UseDebugApi", out object value))
            {
                // Older versions stored this a string
                if (value is bool boolVal)
                    return boolVal;
                else
                    return bool.Parse(value.ToString());
            }
            else
            {
                SetUseDebugApi(true);
                return true;
            }
        }
        public static void SetUseDebugApi(bool value)
        {
            localSettings.Values["UseDebugApi"] = value;
            ApplyUseDebugApi(value);
            UseDebugApiChanged?.Invoke(value);
            SettingsChanged?.Invoke("UseDebugApi", value);
        }
        public static void ApplyUseDebugApi(bool value)
        {
            UwpCommunityBackend.Api.BaseUrl =
                value ?
                UwpCommunityBackend.Api.LOCAL_BASE_URL :
                UwpCommunityBackend.Api.WEB_BASE_URL;
        }
        public delegate void UseDebugApiChangedHandler(bool value);
        public static event UseDebugApiChangedHandler UseDebugApiChanged;

        public static Point GetProjectCardSize()
        {
            if (localSettings.Values.TryGetValue("ProjectCardSize", out object value))
            {
                return (Point)value;
            }
            else
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
            SettingsChanged?.Invoke("ProjectCardSize", value);
        }
        public delegate void ProjectCardSizeChangedHandler(Point value);
        public static event ProjectCardSizeChangedHandler ProjectCardSizeChanged;

        public static bool GetShowLlamaBingo()
        {
            if (localSettings.Values.TryGetValue("ShowLlamaBingo", out object value))
            {
                return (bool)value;
            }
            else
            {
                SetShowLlamaBingo(true);
                return true;
            }
        }
        public static void SetShowLlamaBingo(bool value)
        {
            localSettings.Values["ShowLlamaBingo"] = value;
            ShowLlamaBingoChanged?.Invoke(value);
            SettingsChanged?.Invoke("ShowLlamaBingo", value);
        }
        public delegate void ShowLlamaBingoChangedHandler(bool value);
        public static event ShowLlamaBingoChangedHandler ShowLlamaBingoChanged;

        public static string GetSavedLlamaBingo()
        {
            if (localSettings.Values.TryGetValue("SavedLlamaBingo", out object value))
            {
                return value.ToString();
            }
            else
            {
                SetSavedLlamaBingo(null);
                return null;
            }
        }
        public static void SetSavedLlamaBingo(string boardData)
        {
            localSettings.Values["SavedLlamaBingo"] = boardData;
            SavedLlamaBingoChanged?.Invoke(boardData);
            SettingsChanged?.Invoke("SavedLlamaBingo", boardData);
        }
        public delegate void SavedLlamaBingoChangedHandler(string boardData);
        public static event SavedLlamaBingoChangedHandler SavedLlamaBingoChanged;

        public static bool GetShowLiveTile()
        {
            if (localSettings.Values.TryGetValue("ShowLiveTile", out object value))
            {
                return (bool)value;
            }
            else
            {
                SetShowLiveTile(true);
                return true;
            }
        }
        public static void SetShowLiveTile(bool value)
        {
            localSettings.Values["ShowLiveTile"] = value;
            ApplyLiveTile(value);
            ShowLiveTileChanged?.Invoke(value);
            SettingsChanged?.Invoke("ShowLiveTile", value);
        }
        public static async void ApplyLiveTile(bool value)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();

            if (value)
            {
                // Load all app messages
                try
                {
                    var messages = await YoshiServer.Api.GetAppMessages("UWPCommunity");
                    foreach (YoshiServer.Models.AppMessage message in messages)
                    {
                        if (message.Importance > 1)
                            continue;

                        // Update live tile
                        TileBindingContentAdaptive text = new TileBindingContentAdaptive
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = message.Title,
                                    HintWrap = true,
                                },
                                new AdaptiveText()
                                {
                                    Text = message.Message,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true
                                }
                            }
                        };
                        var tileContent = new TileContent()
                        {
                            Visual = new TileVisual()
                            {
                                TileMedium = new TileBinding()
                                {
                                    Branding = TileBranding.Logo,
                                    Content = text
                                },
                                TileWide = new TileBinding()
                                {
                                    Branding = TileBranding.NameAndLogo,
                                    Content = text
                                },
                                TileLarge = new TileBinding()
                                {
                                    Branding = TileBranding.NameAndLogo,
                                    Content = text
                                }
                            }
                        };
                        var notification = new TileNotification(tileContent.GetXml());
                        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                        TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
                    }
                }
                catch { }
            }
        }
        public delegate void ShowLiveTileChangedHandler(bool value);
        public static event ShowLiveTileChangedHandler ShowLiveTileChanged;

        public static bool GetExtendIntoTitleBar()
        {
            if (localSettings.Values.TryGetValue("ExtendIntoTitleBar", out object value))
            {
                return (bool)value;
            }
            else
            {
                SetExtendIntoTitleBar(true);
                return true;
            }
        }
        public static void SetExtendIntoTitleBar(bool value)
        {
            localSettings.Values["ExtendIntoTitleBar"] = value;
            ExtendIntoTitleBarChanged?.Invoke(value);
            SettingsChanged?.Invoke("ExtendIntoTitleBar", value);
        }
        public delegate void ExtendIntoTitleBarChangedHandler(bool value);
        public static event ExtendIntoTitleBarChangedHandler ExtendIntoTitleBarChanged;

        public static bool GetUseBlurEffects()
        {
            if (localSettings.Values.TryGetValue("UseBlurEffects", out object value))
            {
                return (bool)value;
            }
            else
            {
                SetExtendIntoTitleBar(true);
                return true;
            }
        }
        public static void SetUseBlurEffects(bool value)
        {
            localSettings.Values["UseBlurEffects"] = value;
            UseBlurEffectsChanged?.Invoke(value);
            SettingsChanged?.Invoke("UseBlurEffects", value);
        }
        public delegate void UseBlurEffectsChangedHandler(bool value);
        public static event UseBlurEffectsChangedHandler UseBlurEffectsChanged;

        public delegate void SettingsChangedHandler(string name, object value);
        public static event SettingsChangedHandler SettingsChanged;

        public static class AppMessageSettings
        {
            public static void LoadDefaults(bool overrideCurr = true)
            {
                if (!localSettings.Values.ContainsKey("LastAppMessageId") || overrideCurr)
                    SetLastAppMessageId(null);
                if (!localSettings.Values.ContainsKey("ShowAppMessages") || overrideCurr)
                    SetShowAppMessages(true);
                if (!localSettings.Values.ContainsKey("ImportanceLevel") || overrideCurr)
                    SetImportanceLevel(3);
            }

            public static bool GetShowAppMessages()
            {
                if (localSettings.Values.TryGetValue("ShowAppMessages", out object value))
                {
                    return (bool)value;
                }
                else
                {
                    SetShowAppMessages(true);
                    return true;
                }
            }
            public static void SetShowAppMessages(bool value)
            {
                localSettings.Values["ShowAppMessages"] = value;
                ShowAppMessagesChanged?.Invoke(value);
                SettingsChanged?.Invoke("ShowAppMessages", value);
            }
            public delegate void ShowAppMessagesChangedHandler(bool value);
            public static event ShowAppMessagesChangedHandler ShowAppMessagesChanged;

            public static string GetLastAppMessageId()
            {
                if (localSettings.Values.TryGetValue("LastAppMessageId", out object value))
                {
                    return value.ToString();
                }
                else
                {
                    SetLastAppMessageId(null);
                    return null;
                }
            }
            public static void SetLastAppMessageId(string messageId)
            {
                localSettings.Values["LastAppMessageId"] = messageId;
                LastAppMessageIdChanged?.Invoke(messageId);
                SettingsChanged?.Invoke("LastAppMessageId", messageId);
            }
            public delegate void LastAppMessageIdChangedHandler(string messageId);
            public static event LastAppMessageIdChangedHandler LastAppMessageIdChanged;

            public static int GetImportanceLevel()
            {
                if (localSettings.Values.TryGetValue("ImportanceLevel", out object value))
                {
                    return (int)value;
                }
                else
                {
                    SetImportanceLevel(3);
                    return 3;
                }
            }
            public static void SetImportanceLevel(int level)
            {
                localSettings.Values["ImportanceLevel"] = level;
                ImportanceLevelChanged?.Invoke(level);
                SettingsChanged?.Invoke("ImportanceLevel", level);
            }
            public delegate void ImportanceLevelChangedHandler(int level);
            public static event ImportanceLevelChangedHandler ImportanceLevelChanged;
        }
    }
}
