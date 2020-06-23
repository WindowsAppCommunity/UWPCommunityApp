using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Web;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWPCommunityApp.Controls
{
    public sealed partial class BingoCard : UserControl
    {
        static readonly Version BingoVersion = new Version(App.GetVersion());
        const string fname = @"Assets\LlamaBingo-Tiles.txt";
        static readonly StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        static List<string> AllTiles;

        public BingoCard()
        {
            this.InitializeComponent();
            ResetBoard();
        }
        public BingoCard(string dataString, string boardVersion)
        {
            this.InitializeComponent();
            // TODO: Create a board from data string
            ResetBoard();
            SetByDataString(dataString);
        }

        /// <summary>
        /// Loads tiles from file. Does not reset the board.
        /// </summary>
        public async Task InitBoard()
        {
            if (AllTiles != null)
                return;

            // Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();

            // Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;
            Uri requestUri = new Uri("https://gist.githubusercontent.com/michael-hawker/283fa0ba3577f96e753fde3ac6109618/raw/71f229862e19a60a502af82f3b95a6c9a655f24c/squares.txt");

            // Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                // Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

                // NEVER change the order of the lines in this file.
                // GetBoardAsDataString() relies on the order being constant,
                // so the line number can be used as a unique ID for each tile.
                AllTiles = httpResponseBody.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
        }

        public async void ResetBoard()
        {
            await InitBoard();

            // Clear the board of tiles
            var items = BingoGrid.Children.ToList();
            foreach (UIElement item in items)
            {
                if (item is ToggleButton)
                    BingoGrid.Children.Remove(item);
            }

            // Randomly choose 24 tiles
            var newTiles = GetRandom(AllTiles, 24);
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    // Check if the tile is the free space
                    if (x == 2 && y == 2)
                        continue;

                    int boardIndex = 5 * x + y;
                    if (boardIndex > 12)
                        boardIndex--;
                    SetTile(newTiles.ElementAt(boardIndex), x, y);
                }
            }
            BoardChanged?.Invoke(ToDataString());
        }

        private void SetTile(string text, int x, int y, bool isFilled = false)
        {
            if (x == 2 && y == 2)
            {
                // Attempting to set the FREE space, ignore the request
                return;
            }

            var tileButton = new ToggleButton();
            var tileText = new TextBlock();
            tileText.Style = (Style)Resources["BingoBox"];
            tileText.Text = text;
            tileButton.Content = tileText;
            tileButton.IsChecked = isFilled;

            int boardIndex = 5 * x + y;
            if (boardIndex >= BingoGrid.Children.Count)
                BingoGrid.Children.Add(tileButton);
            else
                BingoGrid.Children.Insert(5 * x + y, tileButton);
        }

        private void AddTile(string text, bool isFilled = false)
        {
            var tileButton = new ToggleButton();
            tileButton.Style = (Style)Resources["BingoTileStyle"];
            var tileText = new TextBlock();
            tileText.Style = (Style)Resources["BingoBox"];
            tileText.Text = text;
            tileButton.Content = tileText;
            tileButton.IsChecked = isFilled;
            BingoGrid.Children.Add(tileButton);
        }

        private (string, bool) GetTile(int x, int y)
        {
            var tileButton = BingoGrid.Children[5 * x + y] as ToggleButton;
            if (tileButton == null)
            {
                // This means the button wasn't found. It is likely the FREE tile.
                return ("FREE", true);
            }
            var tileText = tileButton.Content as TextBlock;
            return (tileText.Text, tileButton.IsChecked.Value);
        }

        private Grid GenerateFreeTile()
        {
            //<Grid Background="{ThemeResource SystemAccentColor}"
            //    Grid.Row="2" Grid.Column="2" x:Name="tileFree">
            //    <TextBlock Style="{StaticResource BingoBox}" FontSize="28">
            //    <Run>🦙</Run><LineBreak/><Run>FREE</Run>
            //    </TextBlock>
            //</Grid>
            return new Grid
            {
                Background = (SolidColorBrush)Resources["AccentBrush"],
                Children =
                {
                    new TextBlock
                    {
                        Style = (Style)Resources["BingoBox"],
                        FontSize = 28,
                        Inlines =
                        {
                            new Run
                            {
                                Text = "Free"
                            },

                            new LineBreak(),

                            new Run
                            {
                                Text = "🦙"
                            }
                        }
                    }
                }
            };
        }

        public static Random randomizer = new Random();

        public static IEnumerable<T> GetRandom<T>(IEnumerable<T> list, int numItems)
        {
            return GetRandom(list as T[] ?? list.ToArray(), numItems);
        }

        public static IEnumerable<T> GetRandom<T>(T[] list, int numItems)
        {
            var items = new HashSet<T>(); // don't want to add the same item twice; otherwise use a list
            while (numItems > 0)
                // if we successfully added it, move on
                if (items.Add(list[randomizer.Next(list.Length)])) numItems--;

            return items;
        }

        /// <summary>
        /// Generates a string that can be used to share the current state of the board
        /// with other users of this app
        /// </summary>
        public string ToDataString()
        {
            // NOTE: When making significant changes to this algorithm,
            // don't delete the code. Create an if branch to run the
            // previous version of the algorithm.

            byte[] tileData = new byte[25];
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    // Get the tile and extract serializable data
                    (string Text, bool IsFilled) tile = GetTile(x, y);
                    int tileIndex = AllTiles.IndexOf(tile.Text) + 1;
                    int isFilled = tile.IsFilled ? 1 : 0;

                    // Encode the tile data as a byte and convert to hex.
                    // The first 7 bits represent the tile id,
                    // the last bit represents whether the tile is filled.
                    byte enc = Convert.ToByte(tileIndex << 1);
                    enc += Convert.ToByte(isFilled);
                    tileData[5 * x + y] = enc;
                }
            }
            return BitConverter.ToString(tileData).Replace("-", string.Empty);
        }

        /// <summary>
        /// Sets the current state of the board to the one specified by the data string
        /// </summary>
        public async void SetByDataString(string dataString, Version boardVersion = null)
        {
            // Check if the version is provided. If not, assume it is the current version.
            if (boardVersion == null)
                boardVersion = BingoVersion;

            await InitBoard();
            ResetBoard();
            BingoGrid.Children.Clear();

            // NOTE: When making significant changes to this algorithm,
            // don't delete the code. Create an if branch to run the
            // previous version of the algorithm.

            byte[] tileData = dataString.Replace("\r", String.Empty).Replace("\n", String.Empty).TakeEvery(2)
                .Select(s => byte.Parse(s, System.Globalization.NumberStyles.HexNumber)).ToArray();
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    var data = tileData[5 * x + y];
                    int textIndex = (data >> 1) - 1;
                    if (textIndex == -1 || (x == 2 && y == 2))
                    {
                        // Tile is free space
                        BingoGrid.Children.Add(GenerateFreeTile());
                        continue;
                    }

                    string text = AllTiles[textIndex];
                    bool isFilled = (data & 1) == 1;
                    AddTile(text, isFilled);
                }
            }
            BoardChanged?.Invoke(dataString);
        }

        public string GetShareLink()
        {
#if DROID
            string encodedData = System.Net.WebUtility.UrlEncode(ToDataString());
#else
            string encodedData = HttpUtility.UrlEncode(ToDataString());
#endif
            return $"uwpcommunity://llamabingo?version={BingoVersion}&board={encodedData}";
        }

        public delegate void BoardChangedHandler(string data);
        public event BoardChangedHandler BoardChanged;
    }
}
