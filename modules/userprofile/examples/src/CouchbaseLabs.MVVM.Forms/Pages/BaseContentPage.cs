using System;
using System.Threading.Tasks;
using CouchbaseLabs.MVVM.Services;
using CouchbaseLabs.MVVM.ViewModels;
using Xamarin.Forms;

namespace CouchbaseLabs.MVVM.Forms.Pages
{
    public abstract class BaseContentPage : ContentPage
    {  }

    public abstract class BaseContentPage<T> : BaseContentPage, IViewFor<T> where T : BaseViewModel
    {
        T _viewModel;

        public T ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                BindingContext = _viewModel = value;

                if (_viewModel != null)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await _viewModel.InitAsync();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    });
                }
            }
        }
         
        object IViewFor.ViewModel
        {
            get => _viewModel;
            set => ViewModel = (T)value;
        }

        protected BaseContentPage()
        { }

        async void Init() => await ViewModel.InitAsync();
    }
}
