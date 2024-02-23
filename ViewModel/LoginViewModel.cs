using MultipleUserLoginForm.Commands;
using MultipleUserLoginForm.Services;
using MultipleUserLoginForm.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MultipleUserLoginForm.Model;
using MultipleUserLoginForm.Stores;
using MultipleUserLoginForm.Views;
using System.ComponentModel;
using System.Collections;
using MultipleUserLoginForm.Validation;

namespace MultipleUserLoginForm.ViewModel
{
    public class LoginViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private string _password;
        private readonly MatricsStore _matricsStore;
        private readonly NavigationStore _navigationStore;
        private readonly ErrorsViewModel _errorsViewModel;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public string Password
        {
            get { return _password; }
            set
            {
                _errorsViewModel.ClearErrors(nameof(Password));
                _errorsViewModel.ClearErrors(nameof(Login));

                _password = value;

                OnPropertyChanged(nameof(Password));
            }
        }
        private string _login;

        

        public LoginViewModel(Stores.NavigationStore navigationStore, MatricsStore ms)
        {
            //ParameterNavigationService<Account, UserCabinetViewModel> ns = new
            //    ParameterNavigationService<Account, UserCabinetViewModel>(navigationStore, (par) => new UserCabinetViewModel(par, navigationStore));
            //LoginCommand = new LoginCommand(this, ns);

            //LoginCommand = new NavigateCommand<AdminViewModel>(new NavigationService<AdminViewModel>(navigationStore,
            //    () => new AdminViewModel(navigationStore)));
            _matricsStore=ms;
            _navigationStore = navigationStore;
            _errorsViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_Errorschanged;
            LoginCommand = new RelayCommand(Navigate,(o)=>CanCreate);
        }

        private void ErrorsViewModel_Errorschanged(object? sender, DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
            OnPropertyChanged(nameof(CanCreate));
        }

        public string Login
        {
            get { return _login; }
            set
            {
                _errorsViewModel.ClearErrors(nameof(Login));
                _errorsViewModel.ClearErrors(nameof(Password));
                _login = value;
                OnPropertyChanged(nameof(Login));
                
                
            }
        }
        public void ValidateInput(object obj)
        {
           


            SubjectViewModel s=_matricsStore.CurrentMatrics.Admins.Find((el)=>el.Login==Login);

            if(s!=null)
            {
                if(s.Password!=Password)
                {
                    _errorsViewModel.AddError(nameof(Password), "Invalid password. Try again");
                    
                    
                }
                return;
            }
            s= _matricsStore.CurrentMatrics.GetSubject(Login);

           
            if (s == null)
            {
               
                _errorsViewModel.AddError(nameof(Login), "There's no a user with such a login");
            }
            else if(s.Password!=Password){
                _errorsViewModel.AddError(nameof(Password), "Invalid password. Try again");

            }
            
            
            
        }
        public void Navigate(object o)
        {
            ValidateInput(o);
            if (CanCreate)
            {
                if(_matricsStore.CurrentMatrics.Admins.Find((s)=>s.Login==Login && s.Password==Password)is not null)
                {
                    NavigateCommand<AdminViewModel> navCom =
                        new NavigateCommand<AdminViewModel>(
                            new NavigationService<AdminViewModel>(_navigationStore,
                            () => new AdminViewModel(_navigationStore,_matricsStore)));
                    navCom.Execute(o);
                }
                else
                {
                    NavigateCommand<UserCabinetViewModel> navCom =
                        new NavigateCommand<UserCabinetViewModel>(
                            new NavigationService<UserCabinetViewModel>(_navigationStore,
                            () => new UserCabinetViewModel(_matricsStore.CurrentMatrics.GetSubject(Login),
                            _navigationStore,_matricsStore)));
                    navCom.Execute(o);
                }
            }
        }
       

        public IEnumerable GetErrors(string? propertyName)
        {
            return 
            _errorsViewModel.GetErrors(propertyName);
        }

        public ICommand LoginCommand { get; }

        public bool HasErrors => _errorsViewModel.HasErrors;
        public bool CanCreate => !HasErrors;

    }
}
