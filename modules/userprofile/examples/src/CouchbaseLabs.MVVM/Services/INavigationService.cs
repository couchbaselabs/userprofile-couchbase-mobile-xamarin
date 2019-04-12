using System;
using System.Reflection;
using System.Threading.Tasks;
using CouchbaseLabs.MVVM.ViewModels;

namespace CouchbaseLabs.MVVM.Services
{
    public interface INavigationService
    {
        void AutoRegister(Assembly asm);
        void Register(Type viewModelType, Type viewType);

        Task PopAsync();
        Task PushAsync(BaseViewModel viewModel);
        Task PushAsync<T>(Action<T> initialize = null) where T : BaseViewModel;
        Task PopToRootAsync(bool animate);

        void ReplaceRoot(BaseViewModel viewModel, bool withNavigationEnabled = true);
    }
}
