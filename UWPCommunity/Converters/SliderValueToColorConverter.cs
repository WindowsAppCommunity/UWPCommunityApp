using System;
using Windows.UI.Xaml.Media;

namespace UWPCommunity.Converters
{
    public class SliderValueToColorConverter
    {
        public static SolidColorBrush Convert(double x) => x switch
        {
            0 => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 232, 17, 35)),
            1 => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 247, 99, 12)),
            2 => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 185, 0)),
            3 => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 204, 106)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
