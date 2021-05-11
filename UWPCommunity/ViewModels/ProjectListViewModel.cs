using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UWPCommunity.ViewModels
{
    public class ProjectListViewModel : ObservableObject
    {
        private List<ProjectViewModel> _AllProjects;
        public List<ProjectViewModel> AllProjects
        {
            get => _AllProjects;
            set => SetProperty(ref _AllProjects, value);
        }

        private ObservableCollection<ProjectViewModel> _Projects = new ObservableCollection<ProjectViewModel>();
        public ObservableCollection<ProjectViewModel> Projects
        {
            get => _Projects;
            set => SetProperty(ref _Projects, value);
        }
    }
}
