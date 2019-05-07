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
        Task PopToRootAsync(bool animate);
        void ReplaceRoot<T>(bool withNavigationEnabled = true) where T : BaseViewModel;
        void ReplaceRoot(BaseViewModel viewModel, bool withNavigationEnabled = true);
    }
}