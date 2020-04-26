using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Web;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class BingoCard : UserControl
    {
        static readonly Version BingoVersion = new Version(1,0,1);
        const string fname = @"Assets\LlamaBingo-Tiles.txt";
        static readonly StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        List<string> AllTiles;

        public BingoCard()
        {
            this.InitializeComponent();
            ResetBoard();
        }
        public BingoCard(string dataString, string boardVersion)
        {
            this.InitializeComponent();
            // TODO: Create a board from data string
            SetByDataString(dataString);
        }

        /// <summary>
        /// Loads tiles from file. Does not reset the board.
        /// </summary>
        public async Task InitBoard()
        {
            if (AllTiles != null)
                return;

            // Get the list of tiles
            StorageFile file = await InstallationFolder.GetFileAsync(fname);
            if (!File.Exists(file.Path))
            {
                throw new FileNotFoundException("Could not find LlamaBingo-Tiles.txt in Assets");
            }
            // NEVER change the order of the lines in this file.
            // GetBoardAsDataString() relies on the order being constant,
            // so the line number can be used as a unique ID for each tile.
            AllTiles = File.ReadAllLines(file.Path).ToList();
        }

        public async void ResetBoard()
        {
            await InitBoard();
            // Randomly choose 24 tiles
            var newTiles = GetRandom(AllTiles, 25);
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    // Check if the tile is the free space
                    if (x == 2 && y == 2)
                        continue;

                    SetTile(newTiles.ElementAt(5 * x + y), x, y);
                }
            }
        }

        private void SetTile(string text, int x, int y, bool isFilled = false)
        {
            if (x == 2 && y == 2)
            {
                // Attempting to set the FREE space, ignore the request
                return;
            }

            var tileButton = this.FindName($"tile{x}{y}") as ToggleButton;
            var tileText = new TextBlock();
            tileText.Style = (Style)Resources["BingoBox"];
            tileText.Text = text;
            tileButton.Content = tileText;
            tileButton.IsChecked = isFilled;
            ((ToggleButton)FindName($"tile{x}{y}")).IsChecked = isFilled;
            ((ToggleButton)FindName($"tile{x}{y}")).Content = tileText;
        }

        private (string, bool) GetTile(int x, int y)
        {
            var tileButton = this.FindName($"tile{x}{y}") as ToggleButton;
            if (tileButton == null)
            {
                // This means the button wasn't found. It is likely the FREE tile.
                return ("FREE", tileFree.IsChecked.Value);
            }
            var tileText = tileButton.Content as TextBlock;
            return (tileText.Text, tileButton.IsChecked.Value);
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
            string[] tileData = new string[25];
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    (string Text, bool IsFilled) tile = GetTile(x, y);
                    int tileIndex = AllTiles.IndexOf(tile.Text);
                    int isFilled = tile.IsFilled ? 1 : 0;
                    tileData[5 * x + y] = $"{tileIndex},{isFilled}";
                }
            }
            return String.Join(";", tileData);
        }

        /// <summary>
        /// Sets the current state of the board to the one specified by the data string
        /// </summary>
        public async void SetByDataString(string dataString)
        {
            await InitBoard();
            string[] tileData = dataString.Split(";");
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    var data = tileData[5 * x + y].Split(",");
                    int textIndex = Int32.Parse(data[0]);
                    if (textIndex == 0)
                        // Tile is free space
                        continue;

                    string text = AllTiles[textIndex];
                    bool isFilled = Int32.Parse(data[1]) == 1;
                    SetTile(text, x, y, isFilled);
                }
            }
        }

        public string GetShareLink()
        {
            return $"uwpcommunity://llamabingo?version={BingoVersion}&board={HttpUtility.UrlEncode(ToDataString())}";
        }
    }
}
