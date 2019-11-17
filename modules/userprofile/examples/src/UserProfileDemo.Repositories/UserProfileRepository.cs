using System;
using System.IO;
using Couchbase.Lite;
using UserProfileDemo.Core;
using UserProfileDemo.Core.Respositories;
using UserProfileDemo.Models;

namespace UserProfileDemo.Respositories
{
    public sealed class UserProfileRepository : BaseRepository<UserProfile, string>, IUserProfileRepository
    {
        // tag::databaseconfiguration[]
        DatabaseConfiguration _databaseConfig;
        protected override DatabaseConfiguration DatabaseConfig
        {
            get
            {
                if (_databaseConfig == null)
                {
                    if (AppInstance.User?.Username == null)
                    {
                        throw new Exception($"Repository Exception: A valid user is required!");
                    }

                    _databaseConfig = new DatabaseConfiguration
                    {
                        Directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                        AppInstance.User.Username)
                    };
                }

                return _databaseConfig;
            }
            set => _databaseConfig = value;
        }
        // end::databaseconfiguration[]

        // tag::databasename[]
        public UserProfileRepository() : base("userprofile")
        { }
        // tag::databasename[]

        // tag::getUserProfile[]
        public override UserProfile Get(string userProfileId)
        // end::getUserProfile[]
        {
            UserProfile userProfile = null;

            try
            {
                // tag::docfetch[]
                var document = Database.GetDocument(userProfileId);

                if (document != null)
                {
                    userProfile = new UserProfile
                    {
                        Id = document.Id,
                        Name = document.GetString("Name"),
                        Email = document.GetString("Email"),
                        Address = document.GetString("Address"),
                        ImageData = document.GetBlob("ImageData")?.Content
                    };
                }
                // end::docfetch[]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UserProfileRepository Exception: {ex.Message}");
            }

            return userProfile;
        }

        // tag::saveUserProfile[]
        public override bool Save(UserProfile userProfile)
        // end::saveUserProfile[]
        {
            try
            {
                if (userProfile != null)
                {
                    // tag::docSet[]
                    var mutableDocument = new MutableDocument(userProfile.Id);
                    mutableDocument.SetString("Name", userProfile.Name);
                    mutableDocument.SetString("Email", userProfile.Email);
                    mutableDocument.SetString("Address", userProfile.Address);
                    mutableDocument.SetString("type", "user");
                    if (userProfile.ImageData != null)
                    {
                        mutableDocument.SetBlob("ImageData", new Blob("image/jpeg", userProfile.ImageData));
                    }
                    // end::docSet[]

                    // tag::docSave[]
                    Database.Save(mutableDocument);
                    // end::docSave[]

                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"UserProfileRepository Exception: {ex.Message}");
            }

            return false;
        }
    }
}


