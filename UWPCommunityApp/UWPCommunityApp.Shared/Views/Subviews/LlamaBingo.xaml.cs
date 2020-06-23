using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Web;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunityApp.Views.Subviews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LlamaBingo : Page
    {
        public static ObservableCollection<string> RecentBoards { get; set; } = new ObservableCollection<string>();

        public LlamaBingo()
        {
            this.InitializeComponent();
            this.DataContext = this;

            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
            {
                CompactOverlayButton.Visibility = Visibility.Visible;
            }
            else
            {
                CompactOverlayButton.Visibility = Visibility.Collapsed;
            }

            Bingo.BoardChanged += Bingo_BoardChanged;

            var savedBoard = SettingsManager.GetSavedLlamaBingo();
            if (savedBoard != null)
            {
                Bingo.SetByDataString(savedBoard);
            }
        }

        private void Bingo_BoardChanged(string data)
        {
            // Save the current board in case of a crash
            SettingsManager.SetSavedLlamaBingo(data);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Llamingo: Board changed",
                new Dictionary<string, string> {
                    { "BoardData", data },
                }
            );
        }

        private async void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            // Render to an image at the current system scale and retrieve pixel contents
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(Bingo);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

            var savePicker = new FileSavePicker();
            savePicker.DefaultFileExtension = ".png";
            savePicker.FileTypeChoices.Add(".png", new List<string> { ".png" });
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.SuggestedFileName = "Llamingo Board.png";

            // Prompt the user to select a file
            var saveFile = await savePicker.PickSaveFileAsync();

            // Verify the user selected a file
            if (saveFile == null)
                return;

            // Encode the image to the selected file on disk
            using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    pixelBuffer.ToArray());

                await encoder.FlushAsync();
            }
        }

        private void CopyLink_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            string boardUrl = Bingo.GetShareLink();
            var boardLink = new Uri(boardUrl);
            DataPackage linkPackage = new DataPackage();
            linkPackage.SetApplicationLink(boardLink);
            //Clipboard.SetContent(linkPackage);

            DataRequest request = args.Request;
            request.Data.SetWebLink(boardLink);
            request.Data.Properties.Title = "Share Board";
            request.Data.Properties.Description = "Share your current Llamingo board";
            request.Data.Properties.ContentSourceApplicationLink = boardLink;
            //request.Data.Properties.Thumbnail = boardLink;

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Llamingo: Board shared",
                new Dictionary<string, string> {
                    { "BoardData", boardUrl },
                }
            );
        }

        public static async Task<WriteableBitmap> RenderUIElement(UIElement element)
        {
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(element);
            var pixelBuffer = await bitmap.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();
            var writeableBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
            using (Stream stream = writeableBitmap.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(pixels, 0, pixels.Length);
            }
            return writeableBitmap;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var queryParams = e.Parameter as NameValueCollection;
            if (queryParams?["board"] != null)
            {
                Bingo.SetByDataString(queryParams["board"]);
            }
            else if (e.Parameter != null)
            {
                // TODO: Sometimes this doesn't work. Maybe consider
                // a different way of keeping the board data
                Bingo.SetByDataString(e.Parameter.ToString());
            }

            CompactOverlayButton.IsChecked = ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay;
        }

        private void ResetBoardButton_Click(object sender, RoutedEventArgs e)
        {
            Bingo.ResetBoard();
            RecentBoards.Insert(0, Bingo.ToDataString());
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Llamingo: Board reset");
        }

        private async void LoadLink_Click(object sender, RoutedEventArgs e)
        {
            var clipboardPackage = Clipboard.GetContent();
            if (clipboardPackage.Contains(StandardDataFormats.Text))
            {
                string link = await clipboardPackage.GetTextAsync();
#if !DROID
                var queries = HttpUtility.ParseQueryString(link);
                if (queries?["board"] != null)
                {
                    Bingo.SetByDataString(queries["board"]);
                    RecentBoards.Insert(0, queries["board"]);
                    return;
                }
#endif
            }

            ContentDialog dialog = new ContentDialog
            {
                Title = "Failed to load board",
                Content = "Your clipboard does not contain a valid link to a board",
                CloseButtonText = "Ok",
                RequestedTheme = SettingsManager.GetAppTheme()
            };
            ContentDialogResult result = await dialog.ShowAsync();
        }

        private async void CompactOverlayButton_Checked(object sender, RoutedEventArgs e)
        {
            var options = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            options.ViewSizePreference = ViewSizePreference.Custom;
            options.CustomSize = new Size(450, 500);
            bool modeSwitched = await ApplicationView.GetForCurrentView()
                .TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, options);
            (Window.Current.Content as Frame).Navigate(typeof(LlamaBingo), Bingo.ToDataString());
        }

        private async void CompactOverlayButton_Unchecked(object sender, RoutedEventArgs e)
        {
            bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
            (Window.Current.Content as Frame).Navigate(typeof(MainPage),
                new Tuple<Type, object>(typeof(LlamaBingo), Bingo.ToDataString()));
        }

        private void RecentBoardsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Bingo.SetByDataString(e.ClickedItem as string);
            // Do this so that the board we just loaded isn't duplicated
            //RecentBoards.RemoveAt(0);
        }
    }
}
