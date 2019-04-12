using CouchbaseLabs.MVVM;
using CouchbaseLabs.MVVM.Services;
using UserProfileDemo.Core.Respositories;
using UserProfileDemo.Core.Services;
using UserProfileDemo.Core.ViewModels;
using UserProfileDemo.Pages;
using UserProfileDemo.Repositories;
using UserProfileDemo.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace UserProfileDemo
{
    public partial class App : Application
    {
        INavigationService NavigationService { get; set; }

        public App()
        {
            InitializeComponent();

            RegisterRepositories();

            RegisterServices();

            /*
            MainPage = new LoginPage
            {
                ViewModel = ServiceContainer.GetInstance<LoginViewModel>()
            };
            */

            NavigationService.ReplaceRoot(ServiceContainer.GetInstance<LoginViewModel>(), false);
        }

        void RegisterRepositories()
        {
            ServiceContainer.Register<IUserProfileRepository>(new UserProfileRepository());
            ServiceContainer.Register<IUniversityRepository>(new UniversityRepository());
        }

        void RegisterServices()
        {
            ServiceContainer.Register<IAlertService>(new AlertService());
            ServiceContainer.Register<IMediaService>(new MediaService());

            NavigationService = new NavigationService();
            NavigationService.AutoRegister(typeof(App).Assembly);

            ServiceContainer.Register(NavigationService);
        }
    }
}
