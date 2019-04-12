using System.Threading.Tasks;

namespace UserProfileDemo.Repositories.Services
{
    public interface IDatabaseSeedService
    {
        Task CopyDatabaseAsync(string targetDirectoryPath);
    }
}
