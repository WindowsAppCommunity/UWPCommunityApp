using System;
using Windows.UI.Xaml.Media;

namespace UWPCommunityApp.Converters
{
    public class SliderValueToColorConverter
    {
        public static SolidColorBrush Convert(double x)
        {
            switch (x)
            {
                case 0:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 232, 17, 35));
                case 1:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 247, 99, 12));
                case 2:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 185, 0));
                case 3:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 204, 106));
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }
    }
}
