using System.Threading.Tasks;

namespace UserProfileDemo.Core.Services
{
    public interface IAlertService
    {
        Task ShowMessage(string title, string message, string cancel);
    }
}
