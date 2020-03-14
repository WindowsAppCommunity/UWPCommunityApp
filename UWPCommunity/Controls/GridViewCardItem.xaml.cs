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
    public sealed partial class GridViewCardItem : UserControl
    {
        public GridViewCardItem()
        {
            this.InitializeComponent();
        }

        #region Access Options
        public bool IsEditable {
            get { return (bool)GetValue(IsEditableProperty); }
            set {
                SetValue(IsEditableProperty, value);
                EditButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(GridViewCardItem), null);

        public bool IsDeletable {
            get { return (bool)GetValue(IsDeletableProperty); }
            set {
                SetValue(IsDeletableProperty, value);
                DeleteButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
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
        #endregion

        #region Commands
        public ICommand EditCommand {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }
        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(GridViewCardItem), null);

        public ICommand DeleteCommand {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }
        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(GridViewCardItem), null);

        public ICommand ViewCommand {
            get { return (ICommand)GetValue(ViewCommandProperty); }
            set { SetValue(ViewCommandProperty, value); }
        }
        public static readonly DependencyProperty ViewCommandProperty =
            DependencyProperty.Register("ViewCommand", typeof(ICommand), typeof(GridViewCardItem), null);
        #endregion
    }
}
