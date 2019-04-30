using System;
using System.Threading.Tasks;
using Couchbase.Lite;
using Couchbase.Lite.Query;
using UserProfileDemo.Core.Respositories;
using UserProfileDemo.Models;

namespace UserProfileDemo.Repositories
{
    public sealed class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository() : base("userprofiles")
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
                            Name = document.GetString("Name"),
                            Email = document.GetString("Email"),
                            Address = document.GetString("Address"),
                            ImageData = document.GetBlob("ImageData")?.Content,
                            University = document.GetString("University")
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
                                userProfile = new UserProfile(); // <2>

                            foreach (var result in e.Results.AllResults())
                                {
                                    var dictionary = result.GetDictionary("userprofile"); // <3>

                                if (dictionary != null)
                                    {
                                        userProfile.Name = dictionary.GetString("name");
                                        userProfile.Email = dictionary.GetString("email");
                                        userProfile.Address = dictionary.GetString("address");
                                        userProfile.University = dictionary.GetString("university");
                                        userProfile.ImageData = dictionary.GetBlob("image")?.Content; // <4>
                                }
                                }

                                userProfileUpdated.Invoke(userProfile);
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
                    mutableDocument.SetString("Name", userProfile.Name);
                    mutableDocument.SetString("Email", userProfile.Email);
                    mutableDocument.SetString("Address", userProfile.Address);
                    mutableDocument.SetString("University", userProfile.University);

                    if (userProfile.ImageData != null)
                    {
                        mutableDocument.SetBlob("ImageData", new Blob("image/jpeg", userProfile.ImageData));
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

        public override void Dispose()
        {
            base.Dispose();

            // Remove the live query change listener
            _userQuery.RemoveChangeListener(_userQueryToken);
        }
    }
}


