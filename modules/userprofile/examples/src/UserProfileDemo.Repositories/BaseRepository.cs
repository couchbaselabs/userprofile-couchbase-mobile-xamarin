using System;
using System.Threading.Tasks;
using Couchbase.Lite;

namespace UserProfileDemo.Repositories
{
    public abstract class BaseRepository : IDisposable
    {
        readonly string _databaseName;
        Database _database;

        protected DatabaseManager _databaseManager;
        protected DatabaseManager DatabaseManager
        {
            get
            {
                if (_databaseManager == null)
                {
                    _databaseManager = new DatabaseManager(_databaseName);
                }

                return _databaseManager;
            }
        }

        protected BaseRepository(string databaseName)
        {
            _databaseName = databaseName;
        }

        protected virtual async Task<Database> GetDatabaseAsync()
        {
            if (_database == null)
            {
                _database = await DatabaseManager?.GetDatabaseAsync();
            }

            return _database;
        }

        public virtual void Dispose()
        {
            if (_database != null)
            {
                _database.Close();
                _database = null;

                // Stop the replicator only after the database has been closed!
                DatabaseManager?.StopReplication();
            }
        }
    }
}
