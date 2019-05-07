using System.Windows.Input;
using CouchbaseLabs.MVVM;
using CouchbaseLabs.MVVM.Input;
using CouchbaseLabs.MVVM.Services;

namespace UserProfileDemo.Core.ViewModels
{
    public class LoginViewModel : BaseNavigationViewModel
    {
        string _username;
        public string Username
        {
            get => _username;
            set
            {
                SetPropertyChanged(ref _username, value);
            }
        }

        string _password;
        public string Password
        {
            get => _password;
            set
            {
                SetPropertyChanged(ref _password, value);
            }
        }

        ICommand _signInCommand;
        public ICommand SignInCommand
        {
            get
            {
                if (_signInCommand == null)
                {
                    _signInCommand = new Command(SignIn);
                }

                return _signInCommand;
            }
        }

        public LoginViewModel(INavigationService navigationService) : base(navigationService)
        {  }

        void SignIn()
        {
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                AppInstance.User = new Models.UserCredentials
                {
                    Username = Username,
                    Password = Password
                };

                Navigation.ReplaceRoot<UserProfileViewModel>();
            }
        }
    } 
}
