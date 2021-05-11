using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPCommunity.ViewModels
{
    public class DashboardViewModel : ObservableObject
    {
        private List<ProjectViewModel> _AllProjects;
        public List<ProjectViewModel> AllProjects
        {
            get => _AllProjects;
            set
            {
                SetProperty(ref _AllProjects, value);
                UpdateSections();
            }
        }


        private ObservableCollection<ProjectViewModel> _ProjectsOwner = new ObservableCollection<ProjectViewModel>();
        public ObservableCollection<ProjectViewModel> ProjectsOwner
        {
            get => _ProjectsOwner;
            private set => SetProperty(ref _ProjectsOwner, value);
        }

        private ObservableCollection<ProjectViewModel> _ProjectsDeveloper = new ObservableCollection<ProjectViewModel>();
        public ObservableCollection<ProjectViewModel> ProjectsDeveloper
        {
            get => _ProjectsDeveloper;
            private set => SetProperty(ref _ProjectsDeveloper, value);
        }

        private ObservableCollection<ProjectViewModel> _ProjectsBetaTester = new ObservableCollection<ProjectViewModel>();
        public ObservableCollection<ProjectViewModel> ProjectsBetaTester
        {
            get => _ProjectsBetaTester;
            private set => SetProperty(ref _ProjectsBetaTester, value);
        }

        private ObservableCollection<ProjectViewModel> _ProjectsTranslator = new ObservableCollection<ProjectViewModel>();
        public ObservableCollection<ProjectViewModel> ProjectsTranslator
        {
            get => _ProjectsTranslator;
            private set => SetProperty(ref _ProjectsTranslator, value);
        }

        private void UpdateSections()
        {
            ProjectsOwner = new ObservableCollection<ProjectViewModel>(AllProjects.Where(p => p.IsOwner));
            ProjectsDeveloper = new ObservableCollection<ProjectViewModel>(AllProjects.Where(p => p.IsDeveloper));
            ProjectsBetaTester = new ObservableCollection<ProjectViewModel>(AllProjects.Where(p => p.IsBetaTester));
            ProjectsTranslator = new ObservableCollection<ProjectViewModel>(AllProjects.Where(p => p.IsTranslator));
        }
    }
}
