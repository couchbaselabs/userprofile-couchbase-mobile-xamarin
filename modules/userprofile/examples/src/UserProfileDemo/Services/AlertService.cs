using System.Threading.Tasks;
using UserProfileDemo.Core.Services;
using Xamarin.Forms;

namespace UserProfileDemo.Services
{
    public class AlertService : IAlertService
    {
        public Task ShowMessage(string title, string message, string cancel) 
            => Application.Current.MainPage.DisplayAlert(title, message, cancel);
    }
}