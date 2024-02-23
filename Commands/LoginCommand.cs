using MultipleUserLoginForm.Services;
using MultipleUserLoginForm.Utilities;
using MultipleUserLoginForm.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.Commands
{
    public class LoginCommand : CommandBase
    {
        private readonly LoginViewModel _loginViewModel;
        private readonly ParameterNavigationService<Account, UserCabinetViewModel> _navigationService;

        public LoginCommand(LoginViewModel loginViewModel, ParameterNavigationService<Account, UserCabinetViewModel> navigationService)
        {
            _loginViewModel = loginViewModel;
            _navigationService = navigationService;
        }

        public override void Execute(object parameter)
        {
            Account account = new Account()
            {
                Email = $"{_loginViewModel.Login}@test.com",
                UserName = _loginViewModel.Login
            };
            //navigate to the account page
            _navigationService.Navigate(account);
        }
    }
}
