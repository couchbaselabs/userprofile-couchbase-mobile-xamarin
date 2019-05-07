using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CouchbaseLabs.MVVM.ViewModels;
using Xamarin.Forms;

namespace CouchbaseLabs.MVVM.Services
{
    public interface IViewFor
    {
        object ViewModel { get; set; }
    }

    public interface IViewFor<T> : IViewFor where T : BaseViewModel
    {
        new T ViewModel { get; set; }
    }

    public class NavigationService : INavigationService
    {
        INavigation FormsNavigation => Application.Current.MainPage.Navigation;

        // View model to view lookup - making the assumption that view model to view will always be 1:1
        readonly Dictionary<Type, Type> _viewModelViewDictionary = new Dictionary<Type, Type>();

        public void AutoRegister(Assembly asm)
        {
            // Loop through everything in the assembly that implements IViewFor<T>
            foreach (var type in asm.DefinedTypes.Where(dt => !dt.IsAbstract &&
                        dt.ImplementedInterfaces.Any(ii => ii == typeof(IViewFor))))
            {
                // Get the IViewFor<T> portion of the type that implements it
                var viewForType = type.ImplementedInterfaces.FirstOrDefault(
                    ii => ii.IsConstructedGenericType &&
                    ii.GetGenericTypeDefinition() == typeof(IViewFor<>));

                // Register it, using the T as the key and the view as the value
                Register(viewForType.GenericTypeArguments[0], type.AsType());

                ServiceContainer.Register(viewForType.GenericTypeArguments[0], viewForType.GenericTypeArguments[0], true);
            }
        }

        public void Register(Type viewModelType, Type viewType)
        {
            if (!_viewModelViewDictionary.ContainsKey(viewModelType))
            {
                _viewModelViewDictionary.Add(viewModelType, viewType);
            }
        }

        public void ReplaceRoot<T>(bool withNavigationEnabled = true) where T : BaseViewModel
        {
            ReplaceRoot(ServiceContainer.GetInstance<T>(), withNavigationEnabled);
        }

        public void ReplaceRoot(BaseViewModel viewModel, bool withNavigationEnabled = true)
        {
            if (InstantiateView(viewModel) is Page view)
            {
                if (withNavigationEnabled)
                {
                    Application.Current.MainPage = new NavigationPage(view);
                }
                else
                {
                    Application.Current.MainPage = view;
                }
            }
        }

        public Task PopAsync() => FormsNavigation.PopAsync(true);

        public Task PopToRootAsync(bool animate) => FormsNavigation.PopToRootAsync(animate);

        public Task PushAsync(BaseViewModel viewModel) => FormsNavigation.PushAsync((Page)InstantiateView(viewModel));

        IViewFor InstantiateView(BaseViewModel viewModel)
        {
            // Figure out what type the view model is
            var viewModelType = viewModel.GetType();

            // Look up what type of view it corresponds to
            var viewType = _viewModelViewDictionary[viewModelType];

            // Instantiate it
            var view = (IViewFor)Activator.CreateInstance(viewType);

            view.ViewModel = viewModel;

            return view;
        }
    }
}