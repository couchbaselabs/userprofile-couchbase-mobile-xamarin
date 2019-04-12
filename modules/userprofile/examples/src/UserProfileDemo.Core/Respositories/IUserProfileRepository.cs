using System;
using System.Threading.Tasks;
using UserProfileDemo.Models;

namespace UserProfileDemo.Core.Respositories
{
    public interface IUserProfileRepository : IDisposable
    {
        Task<UserProfile> GetAsync(string userProfileId);
        Task<bool> SaveAsync(UserProfile userProfile);
    }
}
