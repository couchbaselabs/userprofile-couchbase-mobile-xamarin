using UserProfileDemo.Models;

namespace UserProfileDemo.Core.Respositories
{
    public interface IUserProfileRepository : IRepository<UserProfile, string>
    {
        new UserProfile Get(string userProfileId);
        new bool Save(UserProfile userProfile);
    }
}
