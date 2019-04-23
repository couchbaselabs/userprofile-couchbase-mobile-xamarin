using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProfileDemo.Repositories.Services;
using Windows.ApplicationModel;
using Windows.Storage;

namespace UserProfileDemo.UWP.Services
{
    /// <summary>
    /// The implementation of <see cref="IDatabaseSeedService"/> that copies a prebuilt
    /// database from the Assets folder
    /// </summary>
    public sealed class DatabaseSeedService : IDatabaseSeedService
    {
        #region IDatabaseSeedService

        /// <inheritdoc />
        public async Task CopyDatabaseAsync(string directoryPath)
        {
            var finalPath = Path.Combine(directoryPath, "universities.cblite2");
            Directory.CreateDirectory(finalPath);
            var destFolder = await StorageFolder.GetFolderFromPathAsync(finalPath);
            var assetsFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets\\universities.cblite2");
            var filesList = await assetsFolder.GetFilesAsync();
            foreach (var file in filesList)
            {
                await file.CopyAsync(destFolder);
            }
        }

        #endregion
    }
}
