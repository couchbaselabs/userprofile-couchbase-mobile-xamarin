using System;
using System.Threading.Tasks;
using UserProfileDemo.Models;

namespace UserProfileDemo.Core.Respositories
{
    public interface IUserProfileRepository : IDisposable
    {
        Task<UserProfile> GetAsync(string userProfileId);
        Task<UserProfile> GetAsync(string userProfileId, Action<UserProfile> userProfileUpdate);
        Task<bool> SaveAsync(UserProfile userProfile);
        Task StartReplicationForCurrentUser();
    }
}
