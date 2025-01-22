using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UwpCommunityBackend;
using UwpCommunityBackend.Models;

namespace UWPCommunity.ViewModels
{
    public class LaunchViewModel : ObservableObject
    {
        private ObservableCollection<Project> _LaunchProjects = new ObservableCollection<Project>();
        public ObservableCollection<Project> LaunchProjects
        {
            get => _LaunchProjects;
            set => SetProperty(ref _LaunchProjects, value);
        }

        private int _LaunchYear;
        public int LaunchYear
        {
            get => _LaunchYear;
            set => SetProperty(ref _LaunchYear, value);
        }

        private bool _Loaded;
        public bool Loaded
        {
            get => _Loaded;
            set => SetProperty(ref _Loaded, value);
        }

        private Project _PersistentProject;
        public Project PersistentProject
        {
            get => _PersistentProject;
            set => SetProperty(ref _PersistentProject, value);
        }

        private string _CardTitle;
        public string CardTitle
        {
            get => _CardTitle;
            set => SetProperty(ref _CardTitle, value);
        }

        private string _CardSubtitle;
        public string CardSubtitle
        {
            get => _CardSubtitle;
            set => SetProperty(ref _CardSubtitle, value);
        }

        private string _CardDetails;
        public string CardDetails
        {
            get => _CardDetails;
            set => SetProperty(ref _CardDetails, value);
        }

        public async Task InitializeAsync()
        {
            // Get the card information from the website frontend
            var card = (await Api.GetCard("launch")).Main;
            CardTitle = card.Title;
            CardSubtitle = card.Subtitle;
            CardDetails = string.Join(" ", card.Details);
        }

        public async Task RefreshProjects()
        {
            var launchYear = 2020;
            ICollection<Project> launchProjects = Array.Empty<Project>();

            try
            {
                var launch = await Api.GetLaunchProjects(launchYear);
                launchProjects = launch.Projects;
            }
            catch { }

            LaunchYear = launchYear;
            Loaded = true;
            LaunchProjects = new ObservableCollection<Project>(launchProjects);
        }
    }
}
