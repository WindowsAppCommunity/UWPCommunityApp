using ColorThiefDotNet;
using System;
using System.Collections.Generic;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWPCommunity.Controls
{
    public sealed partial class GridViewCardItem : UserControl, IInvokeProvider
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly ConcurrentDictionary<Uri, Windows.UI.Color> _cachedAccentColors = new ConcurrentDictionary<Uri, Windows.UI.Color>();
        private Brush _defaultImageBackgroundBrush;

        public GridViewCardItem()
        {
            this.InitializeComponent();
        }

        #region Access Options
        public bool IsEditable
        {
            get => (bool)GetValue(IsEditableProperty);
            set
            {
                SetValue(IsEditableProperty, value);
                EditButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                EditMenuButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(GridViewCardItem), null);

        public bool IsDeletable
        {
            get => (bool)GetValue(IsDeletableProperty);
            set
            {
                SetValue(IsDeletableProperty, value);
                DeleteButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                DeleteMenuButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public static readonly DependencyProperty IsDeletableProperty =
            DependencyProperty.Register(nameof(IsDeletable), typeof(bool), typeof(GridViewCardItem), null);
        #endregion

        #region Content
        public string TitleText
        {
            get => (string)GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }
        public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(
            nameof(TitleText), typeof(string), typeof(GridViewCardItem), new PropertyMetadata(string.Empty));

        public Visibility TitleTextVisibility
        {
            get => (Visibility)GetValue(TitleTextVisibilityProperty);
            set => SetValue(TitleTextVisibilityProperty, value);
        }
        public static readonly DependencyProperty TitleTextVisibilityProperty = DependencyProperty.Register(
            nameof(TitleTextVisibility), typeof(Visibility), typeof(GridViewCardItem), new PropertyMetadata(Visibility.Visible));

        public string BodyText
        {
            get => (string)GetValue(BodyTextProperty);
            set => SetValue(BodyTextProperty, value);
        }
        public static readonly DependencyProperty BodyTextProperty = DependencyProperty.Register(
            nameof(BodyText), typeof(string), typeof(GridViewCardItem), new PropertyMetadata(string.Empty));

        public Visibility BodyTextVisibility
        {
            get => (Visibility)GetValue(BodyTextVisibilityProperty);
            set => SetValue(BodyTextVisibilityProperty, value);
        }
        public static readonly DependencyProperty BodyTextVisibilityProperty = DependencyProperty.Register(
            nameof(BodyTextVisibility), typeof(Visibility), typeof(GridViewCardItem), new PropertyMetadata(Visibility.Visible));

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            nameof(ImageSource), typeof(ImageSource), typeof(GridViewCardItem), new PropertyMetadata(null, OnImageSourceChanged));

        public object BadgeContent
        {
            get => (object)GetValue(BadgeContentProperty);
            set => SetValue(BadgeContentProperty, value);
        }
        public static readonly DependencyProperty BadgeContentProperty = DependencyProperty.Register(
            nameof(BadgeContent), typeof(object), typeof(GridViewCardItem), null);

        public Brush ImageBackgroundBrush
        {
            get => (Brush)GetValue(ImageBackgroundBrushProperty);
            set => SetValue(ImageBackgroundBrushProperty, value);
        }
        public static readonly DependencyProperty ImageBackgroundBrushProperty = DependencyProperty.Register(
            nameof(ImageBackgroundBrush), typeof(Brush), typeof(GridViewCardItem), null);
        #endregion

        #region Events
        public delegate void EditRequestedHandler(object p);
        public event EditRequestedHandler EditRequested;
        private void EditButton_Click(object sender, RoutedEventArgs args)
        {
            EditRequested?.Invoke(DataContext);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Proj: Edit",
                new Dictionary<string, string> {
                    { "DataContext", DataContext.ToString() },
                }
            );
        }

        public delegate void DeleteRequestedHandler(object p);
        public event DeleteRequestedHandler DeleteRequested;
        private void DeleteButton_Click(object sender, RoutedEventArgs args)
        {
            DeleteRequested?.Invoke(DataContext);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Proj: Delete",
                new Dictionary<string, string> {
                    { "DataContext", DataContext.ToString() },
                }
            );
        }

        public delegate void ViewRequestedHandler(object p);
        public event ViewRequestedHandler ViewRequested;
        private void ViewButton_Click(object sender, RoutedEventArgs args)
        {
            ViewRequested?.Invoke(DataContext);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Proj: View",
                new Dictionary<string, string> {
                    { "DataContext", DataContext.ToString() },
                }
            );
        }

        public void Invoke()
        {
            ViewRequested?.Invoke(DataContext);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Proj: View",
                new Dictionary<string, string> {
                    { "DataContext", DataContext.ToString() },
                    { "IsFromAutomation", "True" }
                }
            );
        }
        #endregion

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            return;
            var cardItem = (GridViewCardItem)d;
            if (!(e.NewValue is BitmapImage bitmapImage) || bitmapImage.UriSource == null)
            {
                cardItem._defaultImageBackgroundBrush ??= new SolidColorBrush(Colors.Transparent);
                cardItem.ImageBackgroundBrush = cardItem._defaultImageBackgroundBrush;
                return;
            }

            cardItem._defaultImageBackgroundBrush ??= cardItem.ImageBackgroundBrush;
            var uri = bitmapImage.UriSource;

            Task.Run(async () =>
            {
                var accentColor = await GetAccentColorAsync(uri);
                await cardItem.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                {
                    cardItem.ImageBackgroundBrush = new SolidColorBrush(accentColor);
                });
            });
        }

        private static async Task<Windows.UI.Color> GetAccentColorAsync(Uri uri)
        {
            if (_cachedAccentColors.TryGetValue(uri, out var accentColor))
                return accentColor;

            try
            {
                // Open the URI as a stream
                IRandomAccessStream stream;
                if (uri.Scheme == "http" || uri.Scheme == "https")
                {
                    var response = await _httpClient.GetAsync(uri);
                    response.EnsureSuccessStatusCode();

                    var inputStream = await response.Content.ReadAsInputStreamAsync();

                    int streamLength = -1;
                    if (response.Content.TryComputeLength(out var longStreamLength))
                        streamLength = Convert.ToInt32(longStreamLength);

                    var memStream = new MemoryStream(streamLength);
                    await inputStream.AsStreamForRead().CopyToAsync(memStream);
                    stream = memStream.AsRandomAccessStream();

                    inputStream.Dispose();
                }
                else
                {
                    var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                    stream = await file.OpenAsync(FileAccessMode.Read);
                }

                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                var thief = new ColorThief();
                var quantizedAccentColor = await thief.GetColor(decoder);
                stream.Dispose();

                accentColor = new Windows.UI.Color
                {
                    A = quantizedAccentColor.Color.A,
                    R = quantizedAccentColor.Color.R,
                    G = quantizedAccentColor.Color.G,
                    B = quantizedAccentColor.Color.B,
                };
            }
            catch
            {
                accentColor = Colors.Transparent;
            }
            
            _cachedAccentColors.TryAdd(uri, accentColor);
            return accentColor;
        }
    }
}
