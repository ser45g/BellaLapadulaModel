using MultipleUserLoginForm.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.ViewModel
{
    public class MainWindowViewModel:ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        public TitleStore TitleStore { get; set; }=TitleStore.Instance;


        public MainWindowViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
    }
}
