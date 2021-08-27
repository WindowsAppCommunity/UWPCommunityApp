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
        public DashboardViewModel()
        {
            AllProjects.CollectionChanged += AllProjects_CollectionChanged;
        }

        private ObservableCollection<ProjectViewModel> _AllProjects = new ObservableCollection<ProjectViewModel>();
        public ObservableCollection<ProjectViewModel> AllProjects
        {
            get => _AllProjects;
            set
            {
                AllProjects.CollectionChanged -= AllProjects_CollectionChanged;
                SetProperty(ref _AllProjects, value);
                AllProjects.CollectionChanged += AllProjects_CollectionChanged;
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

        private void AllProjects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    if (!(item is ProjectViewModel pvm))
                        continue;

                    if (pvm.IsOwner)
                        ProjectsOwner.Add(pvm);
                    if (pvm.IsDeveloper)
                        ProjectsDeveloper.Add(pvm);
                    if (pvm.IsBetaTester)
                        ProjectsBetaTester.Add(pvm);
                    if (pvm.IsTranslator)
                        ProjectsTranslator.Add(pvm);
                }
            }
        }
    }
}
