using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.System;
using UWPCommLib.Api.Discord;
using UWPCommLib.Api.UWPComm;
using Windows.UI.Xaml.Media;

namespace UWPCommunity
{
    public static class Common
    {
        public static IUwpCommApi UwpCommApi = RestService.For<IUwpCommApi>(
            "https://uwpcommunity-site-backend.herokuapp.com"
        );
        public static IDiscordAPI DiscordApi = RestService.For<IDiscordAPI>(
            "https://discordapp.com/api"
        );

        static string _token;
        public static string DiscordToken
        {
            get {
                return _token;
            }
            set {
                _token = value;
                UwpCommApi = RestService.For<IUwpCommApi>(
                    "https://uwpcommunity-site-backend.herokuapp.com",
                    new RefitSettings()
                    {
                        AuthorizationHeaderValueGetter = () => Task.FromResult(_token)
                    }
                );
                DiscordApi = RestService.For<IDiscordAPI>(
                    "https://discordapp.com/api",
                    new RefitSettings()
                    {
                        AuthorizationHeaderValueGetter = () => Task.FromResult(_token)
                    }
                );
            }
        }
        public static string DiscordRefreshToken { get; set; }

        public static UWPCommLib.Api.Discord.Models.User DiscordUser { get; set; }

        public static bool IsLoggedIn = false;
        public delegate void OnLoginStateChangedHandler(bool isLoggedIn);
        public static event OnLoginStateChangedHandler OnLoginStateChanged;

        public static async Task SignIn(string discordToken, string refreshToken)
        {
            IsLoggedIn = true;
            DiscordToken = discordToken;
            DiscordRefreshToken = refreshToken;
            DiscordUser = await DiscordApi.GetCurrentUser();
            OnLoginStateChanged(IsLoggedIn);
        }
        public static void SignOut()
        {
            IsLoggedIn = false;
            DiscordToken = "";
            DiscordRefreshToken = "";
            DiscordUser = null;
            OnLoginStateChanged(IsLoggedIn);
        }

        public static readonly FontFamily FabricMDL2Assets = new FontFamily("Assets.ttf#Fabric External MDL2 Assets");

        public static bool IsInternetAvailable()
        {
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            return (profile != null) && !String.IsNullOrEmpty(profile.ProfileName);
        }
    }
}
