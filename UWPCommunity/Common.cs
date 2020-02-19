using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using UWPCommLib.Api.Discord;
using UWPCommLib.Api.UWPComm;

namespace UWPCommunity
{
    public static class Common
    {
        public static IUwpCommApi UwpCommApi = RestService.For<IUwpCommApi>(
            "https://uwpcommunity-site-backend.herokuapp.com"
        );
        public static ILoginService DiscordLoginService = RestService.For<ILoginService>(
            "https://discordapp.com/api"
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
                OnLoggedIn();
            }
        }

        public static string DiscordRefreshToken { get; set; }

        public static UWPCommLib.Api.Discord.Models.User DiscordUser { get; set; }

        public static bool IsLoggedIn = false;
        public delegate void OnLoggedInHandler();
        public static event OnLoggedInHandler OnLoggedIn;
    }
}
