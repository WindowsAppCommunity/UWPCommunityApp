using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UnhandledExceptionPage : Page
    {
        public string ExceptionText { get; internal set; }
        public UnhandledExceptionEventArgs Exception { get; internal set; }

        public UnhandledExceptionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Exception = e.Parameter as UnhandledExceptionEventArgs;
            if (Exception != null)
                ExceptionText = Exception.Message + "\n\n" + Exception.Exception.StackTrace; 
        }
    }
}
