using CouchbaseLabs.MVVM.Forms.Pages;
using UserProfileDemo.Core.ViewModels;

namespace UserProfileDemo.Pages
{
    public partial class LoginPage : BaseContentPage<LoginViewModel>
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            userNameEntry.Focus();
        }
    }
}
