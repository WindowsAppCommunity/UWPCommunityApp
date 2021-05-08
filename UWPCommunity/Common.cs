using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.UI.Xaml.Media;

namespace UWPCommunity
{
    public static class Common
    {
        public const string DISCORD_INVITE = "eBHZSKG";
        public const string DISCORD_GUILD_ID = "372137812037730304";


        public static readonly FontFamily SegoeMDL2Assets = new FontFamily("Segoe MDL2 Assets");

        public static bool IsInternetAvailable()
        {
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            return (profile != null) && !String.IsNullOrEmpty(profile.ProfileName);
        }

        public static Windows.UI.Xaml.Visibility BoolToVisibility(bool value)
		{
            return value ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
		}

        public static float RandomBetween(float min, float max)
        {
            return min + (float)new Random().NextDouble() * (max - min);
        }
    }

    public static class StringExtensions
    {
        public static IEnumerable<string> TakeEvery(this string s, int count)
        {
            int index = 0;
            while (index < s.Length)
            {
                if (s.Length - index >= count)
                {
                    yield return s.Substring(index, count);
                }
                else
                {
                    yield return s.Substring(index, s.Length - index);
                }
                index += count;
            }
        }
    }
}
