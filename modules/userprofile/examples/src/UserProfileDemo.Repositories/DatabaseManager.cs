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

        // tag::getDatabase[]
        public async Task<Database> GetDatabaseAsync()
        // end::getDatabase[]
        {
            if (_database == null)
            {
                if (_databaseName == "userprofile")
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
                    // tag::prebuiltdbconfig[]
                    var options = new DatabaseConfiguration();

                    var defaultDirectory = Service.GetInstance<IDefaultDirectoryResolver>().DefaultDirectory();

                    options.Directory = defaultDirectory;
                    // end::prebuiltdbconfig[]

                    if (!Database.Exists(_databaseName, defaultDirectory))
                    {
                        // tag::prebuiltdbnotopen[]
                        // The path to copy the prebuilt database to
                        var databaseSeedService = ServiceContainer.GetInstance<IDatabaseSeedService>();

                        if (databaseSeedService != null)
                        {
                            // Use a (resolved) platform service to copy the database to 'defaultDirectory'
                            await databaseSeedService.CopyDatabaseAsync(defaultDirectory);

                            _database = new Database(_databaseName, options);

                            CreateUniversitiesDatabaseIndexes();
                        }
                        // end::prebuiltdbnotopen[]
                    }
                    else
                    {
                        // tag::prebuiltdbopen[]
                        _database = new Database(_databaseName, options);
                        // end::prebuiltdbopen[]
                    }
                }
            }

            return _database;
        }

        // tag::createUniversityDatabaseIndexes[]
        void CreateUniversitiesDatabaseIndexes()
        {
            _database.CreateIndex("NameLocationIndex",
                                  IndexBuilder.ValueIndex(ValueIndexItem.Expression(Expression.Property("name")),
                                                          ValueIndexItem.Expression(Expression.Property("location"))));
        }
        // end::createUniversityDatabaseIndexes[]
    }
}
