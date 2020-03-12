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
    public sealed partial class GridCard : UserControl
    {
        public GridCard()
        {
            this.InitializeComponent();
        }

        private void Card_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Card_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public string TitleText {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }
        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register("TitleText", typeof(string), typeof(GridView), null);

        public string SubtitleText {
            get { return (string)GetValue(SubtitleTextProperty); }
            set { SetValue(SubtitleTextProperty, value); }
        }
        public static readonly DependencyProperty SubtitleTextProperty =
            DependencyProperty.Register("SubtitleText", typeof(string), typeof(GridView), null);

        public string DescriptionText {
            get { return (string)GetValue(DescriptionTextProperty); }
            set { SetValue(DescriptionTextProperty, value); }
        }
        public static readonly DependencyProperty DescriptionTextProperty =
            DependencyProperty.Register("DescriptionText", typeof(string), typeof(GridView), null);

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
