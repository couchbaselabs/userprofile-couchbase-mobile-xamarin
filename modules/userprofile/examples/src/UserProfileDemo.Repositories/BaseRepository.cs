using System;
using System.Threading.Tasks;
using Couchbase.Lite;

namespace UserProfileDemo.Repositories
{
    public abstract class BaseRepository : IDisposable
    {
        readonly string _databaseName;

        Database _database;

        protected BaseRepository(string databaseName)
        {
            _databaseName = databaseName;
        }

        protected virtual async Task<Database> GetDatabaseAsync()
        {
            if (_database == null)
            {
                var databaseManager = new DatabaseManager(_databaseName);

                if (databaseManager != null)
                {
                    _database = await databaseManager.GetDatabaseAsync();
                }
            }

            return _database;
        }

        // tag::disposeDatabase[]
        public virtual void Dispose()
        // end::disposeDatabase[]
        {
            if (_database != null)
            {
                // tag::dbclose[]
                _database.Close();
                // end::dbclose[]
                _database = null;
            }
        }
    }
}
