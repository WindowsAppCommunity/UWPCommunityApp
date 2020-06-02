using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWPCommunity.Converters
{
    public class SliderValueToStringConverter
    {
        public static string Convert(double x) => x switch
        {
            0 => "Don't show me any messages",
            1 => "Show me messages only about big UWP Communtiy events",
            2 => "Show me changelogs and UWP Community event notifcations",
            3 => "Show me all messages from the developer",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
