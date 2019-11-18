using System;
using System.Threading.Tasks;
using Couchbase.Lite;
using Couchbase.Lite.Query;
using UserProfileDemo.Core;
using UserProfileDemo.Core.Respositories;
using UserProfileDemo.Models;

namespace UserProfileDemo.Repositories
{
    public sealed class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository() : base("userprofile")
        { }

        // Retrieve the UserProfile directly from the database using the userProfileId
        public async Task<UserProfile> GetAsync(string userProfileId)
        {
            UserProfile userProfile = null;

            try
            {
                var database = await GetDatabaseAsync();

                if (database != null)
                {
                    var document = database.GetDocument(userProfileId);

                    if (document != null)
                    {
                        userProfile = new UserProfile
                        {
                            Id = document.Id,
                            Name = document.GetString("name"),
                            Email = document.GetString("email"),
                            Address = document.GetString("address"),
                            ImageData = document.GetBlob("imageData")?.Content,
                            University = document.GetString("university")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UserProfileRepository Exception: {ex.Message}");
            }

            return userProfile;
        }

        IQuery _userQuery;
        ListenerToken _userQueryToken;

        // Retrieve the UserProfile using the QueryBuilder class, and optionally pass an Action to receive Live Query updates
        // tag::getUserLiveQuery[]
        public async Task<UserProfile> GetAsync(string userProfileId, Action<UserProfile> userProfileUpdated)
        // end::getUserLiveQuery[]
        {
            UserProfile userProfile = null;

            try
            {
                var database = await GetDatabaseAsync();

                if (database != null)
                {
                    // tag::livequerybuilder[]
                    _userQuery = QueryBuilder
                                    .Select(SelectResult.All())
                                    .From(DataSource.Database(database))
                                    .Where(Meta.ID.EqualTo(Expression.String(userProfileId))); // <1>
                    // end::livequerybuilder[]

                    if (userProfileUpdated != null)
                    {
                        // tag::livequery[]
                        _userQueryToken = _userQuery.AddChangeListener((object sender, QueryChangedEventArgs e) => // <1>
                        {
                            if (e?.Results != null && e.Error == null)
                            {
                                foreach (var result in e.Results.AllResults())
                                {
                                    var dictionary = result.GetDictionary("userprofile"); // <2>

                                    if (dictionary != null)
                                    {
                                        userProfile = new UserProfile // <3>
                                        {
                                            Name = dictionary.GetString("name"), // <4>
                                            Email = dictionary.GetString("email"),
                                            Address = dictionary.GetString("address"),
                                            University = dictionary.GetString("university"),
                                            ImageData = dictionary.GetBlob("imageData")?.Content
                                        };
                                    }
                                }

                                if (userProfile != null)
                                {
                                    userProfileUpdated.Invoke(userProfile);
                                }
                            }
                        });
                        // end::livequery[]
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UserProfileRepository Exception: {ex.Message}");
            }

            return userProfile;
        }

        public async Task<bool> SaveAsync(UserProfile userProfile)
        {
            try
            {
                if (userProfile != null)
                {
                    var mutableDocument = new MutableDocument(userProfile.Id);
                    mutableDocument.SetString("name", userProfile.Name);
                    mutableDocument.SetString("email", userProfile.Email);
                    mutableDocument.SetString("address", userProfile.Address);
                    mutableDocument.SetString("university", userProfile.University);
                    mutableDocument.SetString("type", "user");

                    if (userProfile.ImageData != null)
                    {
                        mutableDocument.SetBlob("imageData", new Blob("image/jpeg", userProfile.ImageData));
                    }

                    var database = await GetDatabaseAsync();

                    database.Save(mutableDocument);

                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"UserProfileRepository Exception: {ex.Message}");
            }

            return false;
        }

        public Task StartReplicationForCurrentUser()
        {
            return Task.Run(async () => await DatabaseManager.StartReplicationAsync(AppInstance.User.Username,
                                                                                    AppInstance.User.Password,
                                                                                    new string[] { AppInstance.User.Username }));
        }

        public override void Dispose()
        {
            // Remove the live query change listener
            _userQuery.RemoveChangeListener(_userQueryToken);

            base.Dispose();
        }
    }
}


