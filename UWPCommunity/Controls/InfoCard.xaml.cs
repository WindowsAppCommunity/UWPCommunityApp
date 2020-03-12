using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWPCommunity.Controls
{
    public sealed partial class InfoCard : UserControl
    {
        public delegate void ButtonClickedHandler(object sender, RoutedEventArgs e);
        public event ButtonClickedHandler ButtonClicked;

        public InfoCard()
        {
            this.InitializeComponent();
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked(sender, e);
        }

        private void Card_PointerEntered(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Card_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            
        }

        public string TitleText {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }
        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register("TitleText", typeof(string), typeof(GridView), null);

        public Visibility TitleTextVisibility {
            get { return (Visibility)GetValue(TitleTextVisibilityProperty); }
            set { SetValue(TitleTextVisibilityProperty, value); }
        }
        public static readonly DependencyProperty TitleTextVisibilityProperty =
            DependencyProperty.Register("TitleTextVisibility", typeof(Visibility), typeof(GridView), null);

        public string SubtitleText {
            get { return (string)GetValue(SubtitleTextProperty); }
            set { SetValue(SubtitleTextProperty, value); }
        }
        public static readonly DependencyProperty SubtitleTextProperty =
            DependencyProperty.Register("SubtitleText", typeof(string), typeof(GridView), null);

        public Visibility SubtitleTextVisibility {
            get { return (Visibility)GetValue(SubtitleTextVisibilityProperty); }
            set { SetValue(SubtitleTextVisibilityProperty, value); }
        }
        public static readonly DependencyProperty SubtitleTextVisibilityProperty =
            DependencyProperty.Register("SubtitleTextVisibility", typeof(Visibility), typeof(GridView), null);

        public string BodyText {
            get { return (string)GetValue(BodyTextProperty); }
            set { SetValue(BodyTextProperty, value); }
        }
        public static readonly DependencyProperty BodyTextProperty =
            DependencyProperty.Register("BodyText", typeof(string), typeof(GridView), null);

        public Visibility BodyTextVisibility {
            get { return (Visibility)GetValue(BodyTextVisibilityProperty); }
            set { SetValue(BodyTextVisibilityProperty, value); }
        }
        public static readonly DependencyProperty BodyTextVisibilityProperty =
            DependencyProperty.Register("BodyTextVisibility", typeof(Visibility), typeof(GridView), null);

        public string ButtonText {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(GridView), null);

        public ICommand ButtonCommand {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }
        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(GridView), null);

        public ImageSource ImageSource {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("CardImageSource", typeof(ImageSource), typeof(GridView), null);
    }
}
