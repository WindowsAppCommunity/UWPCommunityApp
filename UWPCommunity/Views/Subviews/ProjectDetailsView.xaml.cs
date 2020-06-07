using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPCommLib.Api.UWPComm.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views.Subviews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectDetailsView : Page
    {
        public ProjectDetailsView()
        {
            this.InitializeComponent();
        }

        public Project Project { get; set; }
        private Type PreviousPage;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ConnectedAnimation animation =
                ConnectedAnimationService.GetForCurrentView().GetAnimation("projectView");
            if (animation != null)
            {
                animation.TryStart(HeroImageCtl);
            }
            PreviousPage = e.SourcePageType;

            var project = e.Parameter as Project;
            if (project != null)
            {
                Project = project;

                // Set up the CollaboratorsBlock, since it can't be done with
                // just bindings
                CollaboratorsBlock.Text += String.Join(", ",
                    Project.Collaborators.Select(c => c.Name));
            }

            base.OnNavigatedTo(e);
        }

        private async void ExternalLinkButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigationManager.OpenInBrowser(Project.ExternalLink);
        }

        private async void GitHubLinkButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigationManager.OpenInBrowser(Project.GitHubLink);
        }

        private async void DownloadLinkButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigationManager.OpenInBrowser(Project.DownloadLink);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.PageFrame.GoBack();
        }
    }
}
