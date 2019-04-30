using System.Threading.Tasks;
using CouchbaseLabs.MVVM;
using CouchbaseLabs.MVVM.Services;
using CouchbaseLabs.MVVM.ViewModels;

namespace UserProfileDemo.Core.ViewModels
{
    public abstract class BaseNavigationViewModel : BaseViewModel
    {
        protected INavigationService Navigation { get; set; }

        protected BaseNavigationViewModel(INavigationService navigationService)
        {
            Navigation = navigationService; 
        }

        public Task Dismiss() => Navigation.PopAsync();
    }
}
