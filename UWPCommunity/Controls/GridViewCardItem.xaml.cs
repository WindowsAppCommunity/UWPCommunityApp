using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWPCommunity.Controls
{
    public sealed partial class GridViewCardItem : UserControl, IInvokeProvider
    {
        public GridViewCardItem()
        {
            this.InitializeComponent();
            //DataContextChanged += (sender, args) =>
            //{
            //    if (args.NewValue != null) Bindings.Update();
            //};
        }

        #region Access Options
        public bool IsEditable {
            get { return (bool)GetValue(IsEditableProperty); }
            set {
                SetValue(IsEditableProperty, value);
                EditButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                EditMenuButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(GridViewCardItem), null);

        public bool IsDeletable {
            get { return (bool)GetValue(IsDeletableProperty); }
            set {
                SetValue(IsDeletableProperty, value);
                DeleteButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                DeleteMenuButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public static readonly DependencyProperty IsDeletableProperty =
            DependencyProperty.Register("IsDeletable", typeof(bool), typeof(GridViewCardItem), null);
        #endregion

        #region Content
        public string TitleText {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }
        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register("TitleText", typeof(string), typeof(GridViewCardItem), null);

        public Visibility TitleTextVisibility {
            get { return (Visibility)GetValue(TitleTextVisibilityProperty); }
            set { SetValue(TitleTextVisibilityProperty, value); }
        }
        public static readonly DependencyProperty TitleTextVisibilityProperty =
            DependencyProperty.Register("TitleTextVisibility", typeof(Visibility), typeof(GridViewCardItem), null);

        public string BodyText {
            get { return (string)GetValue(BodyTextProperty); }
            set { SetValue(BodyTextProperty, value); }
        }
        public static readonly DependencyProperty BodyTextProperty =
            DependencyProperty.Register("BodyText", typeof(string), typeof(GridViewCardItem), null);

        public Visibility BodyTextVisibility {
            get { return (Visibility)GetValue(BodyTextVisibilityProperty); }
            set { SetValue(BodyTextVisibilityProperty, value); }
        }
        public static readonly DependencyProperty BodyTextVisibilityProperty =
            DependencyProperty.Register("BodyTextVisibility", typeof(Visibility), typeof(GridViewCardItem), null);

        public ImageSource ImageSource {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("CardImageSource", typeof(ImageSource), typeof(GridViewCardItem), null);

        public object BadgeContent {
            get { return (object)GetValue(BadgeContentProperty); }
            set { SetValue(BadgeContentProperty, value); }
        }
        public static readonly DependencyProperty BadgeContentProperty =
            DependencyProperty.Register("BadgeContent", typeof(object), typeof(GridViewCardItem), null);
        #endregion

        #region Events
        public delegate void EditRequestedHandler(object p);
        public event EditRequestedHandler EditRequested;
        private void EditButton_Click(object sender, RoutedEventArgs args)
        {
            EditRequested?.Invoke(DataContext);
        }

        public delegate void DeleteRequestedHandler(object p);
        public event DeleteRequestedHandler DeleteRequested;
        private void DeleteButton_Click(object sender, RoutedEventArgs args)
        {
            DeleteRequested?.Invoke(DataContext);
        }

        public delegate void ViewRequestedHandler(object p);
        public event ViewRequestedHandler ViewRequested;
        private void ViewButton_Click(object sender, RoutedEventArgs args)
        {
            ViewRequested?.Invoke(DataContext);
        }

        public void Invoke()
        {
            ViewRequested?.Invoke(DataContext);
        }
        #endregion
    }
}
