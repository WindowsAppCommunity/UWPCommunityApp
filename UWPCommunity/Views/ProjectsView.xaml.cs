using Refit;
using System.Collections.ObjectModel;
using UWPCommLib.Api.UWPComm.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectsView : Page
    {
        UWPCommLib.Api.UWPComm.IUWPCommAPI api = RestService.For<UWPCommLib.Api.UWPComm.IUWPCommAPI>("https://uwpcommunity-site-backend.herokuapp.com");
        public ObservableCollection<Project> Projects { get; set; } = new ObservableCollection<Project>();

        public ProjectsView()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var projs = await api.GetProjects();
            foreach (var project in projs)
            {
                Projects.Add(project);
            }
            base.OnNavigatedTo(e);
        }
    }
}
