using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Couchbase.Lite;
using Couchbase.Lite.DI;
using Couchbase.Lite.Query;
using CouchbaseLabs.MVVM;
using UserProfileDemo.Core;
using UserProfileDemo.Repositories.Services;

namespace UserProfileDemo.Repositories
{
    internal class DatabaseManager
    {
        readonly string _databaseName;

        Database _database;

        internal DatabaseManager(string databaseName)
        {
            _databaseName = databaseName;
        }

        public async Task<Database> GetDatabaseAsync()
        {
            if (_database == null)
            {
                if (_databaseName == "userprofiles")
                {
                    var databaseConfig = new DatabaseConfiguration
                    {
                        Directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                        AppInstance.User.Username)
                    };

                    _database = new Database(_databaseName, databaseConfig);
                }
                else if (_databaseName == "universities")
                {
                    var options = new DatabaseConfiguration();

                    var defaultDirectory = Service.GetInstance<IDefaultDirectoryResolver>().DefaultDirectory();

                    options.Directory = defaultDirectory;

                    if (!Database.Exists(_databaseName, defaultDirectory))
                    {
                        // Load prebuilt database to path
                        var copier = ServiceContainer.GetInstance<IDatabaseSeedService>();

                        if (copier != null)
                        {
                            await copier.CopyDatabaseAsync(defaultDirectory);

                            _database = new Database(_databaseName, options);

                            CreateUniversitiesDatabaseIndexes();
                        }
                    }
                    else
                    {
                        _database = new Database(_databaseName, options);
                    }
                }
            }

            return _database;
        }

        void CreateUniversitiesDatabaseIndexes()
        {
            _database.CreateIndex("NameLocationIndex",
                                  IndexBuilder.ValueIndex(ValueIndexItem.Expression(Expression.Property("name")),
                                                          ValueIndexItem.Expression(Expression.Property("location"))));
        }
    }
}
