using System;

namespace UWPCommunityApp.Converters
{
    public class SliderValueToStringConverter
    {
        public static string Convert(double x)
        {
            switch (x)
            {
                case 0:
                    return "Don't show me any messages";
                case 1:
                    return "Show me messages only about big UWP Community events";
                case 2:
                    return "Show me changelogs and UWP Community event notifcations";
                case 3:
                    return "Show me all messages from the developer";
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }
    }
}
