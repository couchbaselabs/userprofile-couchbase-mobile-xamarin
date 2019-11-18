using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Lite;
using Couchbase.Lite.DI;
using Couchbase.Lite.Query;
using Couchbase.Lite.Sync;
using CouchbaseLabs.MVVM;
using UserProfileDemo.Core;
using UserProfileDemo.Repositories.Services;

namespace UserProfileDemo.Repositories
{
    public class DatabaseManager : IDisposable
    {
        // Note: User 'localhost' when using a simulator
        readonly Uri _remoteSyncUrl = new Uri("ws://localhost:4984");

        // Note: Use '10.0.2.2' when using an emulator
        //readonly Uri _remoteSyncUrl = new Uri("ws://10.0.2.2:4984");

        readonly string _databaseName;

        Replicator _replicator;
        ListenerToken _replicatorListenerToken;

        Database _database;

        public DatabaseManager(string databaseName)
        {
            _databaseName = databaseName;
        }

        public async Task<Database> GetDatabaseAsync()
        {
            if (_database == null)
            {
                if (_databaseName == "userprofile")
                {
                    var defaultDirectory = Service.GetInstance<IDefaultDirectoryResolver>().DefaultDirectory();

                    var databaseConfig = new DatabaseConfiguration
                    {
                        Directory = Path.Combine(defaultDirectory, AppInstance.User.Username)
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
                        // The path to copy the prebuilt database to
                        var databaseSeedService = ServiceContainer.GetInstance<IDatabaseSeedService>();

                        if (databaseSeedService != null)
                        {
                            // Use a (resolved) platform service to copy the database to 'defaultDirectory'
                            await databaseSeedService.CopyDatabaseAsync(defaultDirectory);

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

        // tag::startSync[]
        public async Task StartReplicationAsync(string username,
                                                string password,
                                                string[] channels,
                                                ReplicatorType replicationType = ReplicatorType.PushAndPull,
                                                bool continuous = true)
        // end::startSync[]
        {
            var database = await GetDatabaseAsync();

            var targetUrlEndpoint = new URLEndpoint(new Uri(_remoteSyncUrl, _databaseName));

            // tag::replicationconfig[]
            var configuration = new ReplicatorConfiguration(database, targetUrlEndpoint) // <1>
            {
                ReplicatorType = replicationType, // <2>
                Continuous = continuous, // <3>
                Authenticator = new BasicAuthenticator(username, password), // <4>
                Channels = channels?.Select(x => $"channel.{x}").ToArray() // <5>
            };
            // end::replicationconfig[]

            // tag::replicationinit[]
            _replicator = new Replicator(configuration);
            // end::replicationinit[]

            // tag::replicationlistener[]
            _replicatorListenerToken = _replicator.AddChangeListener(OnReplicatorUpdate);
            // end::replicationlistener[]

            // tag::replicationstart[]
            _replicator.Start();
            // end::replicationstart[]
        }

        // tag::replicatorupdate[]
        void OnReplicatorUpdate(object sender, ReplicatorStatusChangedEventArgs e)
        {
            var status = e.Status;

            switch (status.Activity)
            {
                case ReplicatorActivityLevel.Busy:
                    Console.WriteLine("Busy transferring data.");
                    break;
                case ReplicatorActivityLevel.Connecting:
                    Console.WriteLine("Connecting to Sync Gateway.");
                    break;
                case ReplicatorActivityLevel.Idle:
                    Console.WriteLine("Replicator in idle state.");
                    break;
                case ReplicatorActivityLevel.Offline:
                    Console.WriteLine("Replicator in offline state.");
                    break;
                case ReplicatorActivityLevel.Stopped:
                    Console.WriteLine("Completed syncing documents.");
                    break;
            }

            if (status.Progress.Completed == status.Progress.Total)
            {
                Console.WriteLine("All documents synced.");
            }
            else
            {
                Console.WriteLine($"Documents {status.Progress.Total - status.Progress.Completed} still pending sync");
            }
        }
        // end::replicatorupdate[]

        // tag::replicationstop[]
        public void StopReplication()
        // end::replicationstop[]
        {
            // tag::replicationcleanup[]
            _replicator.RemoveChangeListener(_replicatorListenerToken);
            _replicator.Stop();
            // end::replicationcleanup[]
        }

        public void Dispose()
        {
            if (_replicator != null)
            {
                StopReplication();

                // Because the 'Stop' method for a Replicator instance is asynchronous 
                // we must wait for the status activity to be stopped before closing the database. 
                while (true)
                {
                    if (_replicator.Status.Activity == ReplicatorActivityLevel.Stopped)
                    {
                        break;
                    }
                }

                _replicator.Dispose();
            }

            _database.Close();
            _database = null;
        }
    }
}
