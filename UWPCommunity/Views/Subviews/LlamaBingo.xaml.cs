using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views.Subviews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LlamaBingo : Page
    {
        public LlamaBingo()
        {
            this.InitializeComponent();
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
            DataPackage linkPackage = new DataPackage();
            linkPackage.SetText(Bingo.GetShareLink());
            Clipboard.SetContent(linkPackage);
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

            var queryParams = e.Parameter as Dictionary<string, string>;
            if (queryParams != null && queryParams.ContainsKey("board"))
            {
                Version boardVersion = queryParams.ContainsKey("version") ? new Version(queryParams["version"]) : null;
                Bingo.SetByDataString(queryParams["board"], boardVersion);
            }
        }

        private void ResetBoardButton_Click(object sender, RoutedEventArgs e)
        {
            Bingo.ResetBoard();
        }

        private async void LoadLink_Click(object sender, RoutedEventArgs e)
        {
            string link = await Clipboard.GetContent().GetTextAsync();
            var queries = new Uri(link).DecodeQueryParameters();
            if (queries?["board"] != null)
            {
                Bingo.SetByDataString(queries["board"]);
            }
        }
    }
}
