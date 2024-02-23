using MultipleUserLoginForm.Model;
using MultipleUserLoginForm.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.Stores
{
    public class MatricsStore
    {
        public event Action CurrentMatricsChanged;
        private MatricesViewModel _currentMatrics;
        public MatricesViewModel CurrentMatrics
        {
            get => _currentMatrics; set
            {
                _currentMatrics  = value;
                OnCurrentViewModelChanged();
            }
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentMatricsChanged?.Invoke();
        }
    }
    
}
