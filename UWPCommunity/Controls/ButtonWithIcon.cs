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

        public string IconSpacing {
            get { return (string)GetValue(IconSpacingProperty); }
            set { SetValue(IconSpacingProperty, value); }
        }
        public static readonly DependencyProperty IconSpacingProperty =
            DependencyProperty.Register("IconSpacing", typeof(int), typeof(ButtonWithIcon), null);

        public string Glyph {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }
        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register("Glyph", typeof(string), typeof(ButtonWithIcon), null);

        public FontFamily GlyphFont {
            get { return (FontFamily)GetValue(GlyphFontProperty); }
            set { SetValue(GlyphFontProperty, value); }
        }
        public static readonly DependencyProperty GlyphFontProperty =
            DependencyProperty.Register("GlyphFont", typeof(FontFamily), typeof(ButtonWithIcon), null);

    }
}
