using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomeView : Page
    {
        public HomeView()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
            {
                // Allows the cards to cast a shadow on the background
                CardThemeShadow.Receivers.Add(BackgroundGrid);
            }
        }

        private void Card_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
            {
                // Raise the Z translation so the ThemeShadow gets 'larger'
                ((Grid)sender).Translation = new System.Numerics.Vector3(0, 0, 70);
            }
        }

        private void Card_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
            {
                // Lower the Z translation so the ThemeShadow gets 'smaller'
                ((Grid)sender).Translation = new System.Numerics.Vector3(0, 0, 32);
            }
        }
    }
}
