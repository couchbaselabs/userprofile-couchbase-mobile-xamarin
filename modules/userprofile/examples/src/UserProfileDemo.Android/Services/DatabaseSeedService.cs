using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Android.Content;
using UserProfileDemo.Repositories.Services;

namespace UserProfileDemo.Droid.Services
{
    /// <summary>
    /// An implementation of <see cref="IDatabaseSeedService"/> that reads a prebuilt
    /// database from Android Assets
    /// </summary>
    public class DatabaseSeedService : IDatabaseSeedService
    {
        #region Variables

        private readonly Context _context;

        #endregion

        #region Constructors

        public DatabaseSeedService(Context context)
        {
            _context = context;
        }

        #endregion

        #region IDatabaseSeedService

        public async Task CopyDatabaseAsync(string targetDirectoryPath)
        {
            Directory.CreateDirectory(targetDirectoryPath);

            var assetStream = _context.Assets.Open("universities.zip");

            using (var archive = new ZipArchive(assetStream, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    var entryPath = Path.Combine(targetDirectoryPath, entry.FullName);

                    if (entryPath.EndsWith("/"))
                    {
                        Directory.CreateDirectory(entryPath);
                    }
                    else
                    {
                        using (var entryStream = entry.Open())
                        using (var writeStream = File.OpenWrite(entryPath))
                        {
                            await entryStream.CopyToAsync(writeStream).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
