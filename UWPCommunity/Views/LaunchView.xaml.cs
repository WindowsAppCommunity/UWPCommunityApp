using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using UWPCommLib.Api.UWPComm.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchView : Page
    {
        public ObservableCollection<Project> LaunchProjects { get; set; } = new ObservableCollection<Project>();
        public Project PersistantProject;

        public LaunchView()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!Common.IsInternetAvailable()) return;

            var projs = (await Common.UwpCommApi.GetProjects()).FindAll((project) => project.LaunchYear == DateTime.Now.Year && project.IsAwaitingLaunchApproval == false);
            foreach (var project in projs)
            {
                LaunchProjects.Add(project);
            }
            LoadingIndicator.Visibility = Visibility.Collapsed;
            PersistantProject = e.Parameter as Project;
            base.OnNavigatedTo(e);
        }

        private void Card_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Storyboard sb = ((DropShadowPanel)sender).Resources["EnterStoryboard"] as Storyboard;
            sb.Begin();
        }

        private void Card_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Storyboard sb = ((DropShadowPanel)sender).Resources["ExitStoryboard"] as Storyboard;
            sb.Begin();
        }

        private void Launch2020Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateToDashboard();
        }

        private void ParticipantsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Project item = ParticipantsGridView.SelectedItem as Project;
            ParticipantsGridView.PrepareConnectedAnimation("projectView", item, "HeroImageStartCtl");
            NavigationManager.NavigateToViewProject(item);
        }

        private async void ParticipantsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (PersistantProject != null)
            {
                ParticipantsGridView.ScrollIntoView(PersistantProject);
                ConnectedAnimation animation =
                    ConnectedAnimationService.GetForCurrentView().GetAnimation("projectView");
                if (animation != null)
                {
                    await ParticipantsGridView.TryStartConnectedAnimationAsync(
                        animation, PersistantProject, "HeroImageStartCtl");
                }
            }
        }
    }
}
