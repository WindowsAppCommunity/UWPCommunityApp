using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace UWPCommunity.Controls
{
    public sealed class ButtonWithIcon : Button
    {
        public ButtonWithIcon()
        {
            this.DefaultStyleKey = typeof(ButtonWithIcon);
        }

        public double IconSpacing {
            get { return (double)GetValue(IconSpacingProperty); }
            set { SetValue(IconSpacingProperty, value); }
        }
        public static readonly DependencyProperty IconSpacingProperty =
            DependencyProperty.Register(nameof(IconSpacing), typeof(double), typeof(ButtonWithIcon), new PropertyMetadata(4));

        public IconElement Icon {
            get { return (IconElement)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(ButtonWithIcon), null);

    }
}
