using System.Threading.Tasks;
using CouchbaseLabs.MVVM.Services;

namespace CouchbaseLabs.MVVM.ViewModels
{
    public abstract class BaseViewModel : BaseNotify
    {
        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetPropertyChanged(ref _isBusy, value);
        }

        public virtual Task InitAsync() => Task.FromResult(true);
    }
}
