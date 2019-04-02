using UserProfileDemo.Core;
using UserProfileDemo.Core.Respositories;
using UserProfileDemo.Core.Services;
using UserProfileDemo.Pages;
using UserProfileDemo.Respositories;
using UserProfileDemo.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace UserProfileDemo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set up in place of having a dependency on a DI solution
            RegisterServices();
            RegisterRepositories();

            MainPage = new LoginPage(OnSignInSuccessful);
        }

        void RegisterServices()
        {
            ServiceContainer.Register<IAlertService>(() => new AlertService());
            ServiceContainer.Register<IMediaService>(() => new MediaService());
        }

        void RegisterRepositories()
        {
            ServiceContainer.Register<IUserProfileRepository>(() => new UserProfileRepository());
        }

        void OnSignInSuccessful() => MainPage = new NavigationPage(new UserProfilePage(OnLogoutSuccesful));

        void OnLogoutSuccesful() => MainPage = new LoginPage(OnSignInSuccessful);
    }
}
