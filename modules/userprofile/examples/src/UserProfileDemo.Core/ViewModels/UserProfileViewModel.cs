using System;
using System.Threading.Tasks;
using System.Windows.Input;
using UserProfileDemo.Core.Respositories;
using UserProfileDemo.Core.Services;
using UserProfileDemo.Models;

namespace UserProfileDemo.Core.ViewModels
{
    public class UserProfileViewModel : BaseViewModel
    {
        Action LogoutSuccessful { get; set; }

        IUserProfileRepository UserProfileRepository { get; set; }
        IAlertService AlertService { get; set; }
        IMediaService MediaService { get; set; }

        // tag::userProfileDocId[]
        string UserProfileDocId => $"user::{AppInstance.User.Username}";
        // end::userProfileDocId[]

        string _name;
        public string Name
        {
            get => _name;
            set => SetPropertyChanged(ref _name, value);
        }

        string _email;
        public string Email
        {
            get => _email;
            set => SetPropertyChanged(ref _email, value);
        }

        string _address;
        public string Address
        {
            get => _address;
            set => SetPropertyChanged(ref _address, value);
        }

        byte[] _imageData;
        public byte[] ImageData
        {
            get => _imageData;
            set => SetPropertyChanged(ref _imageData, value);
        }

        ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new Command(async() => await Save());
                }

                return _saveCommand;
            }
        }

        ICommand _selectImageCommand;
        public ICommand SelectImageCommand
        {
            get
            {
                if (_selectImageCommand == null)
                {
                    _selectImageCommand = new Command(async () => await SelectImage());
                }

                return _selectImageCommand;
            }
        }

        ICommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                if (_logoutCommand == null)
                {
                    _logoutCommand = new Command(Logout);
                }

                return _logoutCommand;
            }
        }

        public UserProfileViewModel(IUserProfileRepository userProfileRepository, IAlertService alertService, 
                                    IMediaService mediaService, Action logoutSuccessful)
        {
            UserProfileRepository = userProfileRepository;
            AlertService = alertService;
            MediaService = mediaService;
            LogoutSuccessful = logoutSuccessful;

            LoadUserProfile();
        }

        async void LoadUserProfile()
        {
            IsBusy = true;

            var userProfile = await Task.Run(() =>
            {
                // tag::getUserProfileUsingRepo[]
                var up = UserProfileRepository?.Get(UserProfileDocId);
                // end::getUserProfileUsingRepo[]

                if (up == null)
                {
                    up = new UserProfile
                    {
                        Id = UserProfileDocId,
                        Email = AppInstance.User.Username
                    };
                }

                return up;
            });

            if (userProfile != null)
            {
                Name = userProfile.Name;
                Email = userProfile.Email;
                Address = userProfile.Address;
                ImageData = userProfile.ImageData;
            }

            IsBusy = false;
        }

        Task Save()
        {
            var userProfile = new UserProfile
            {
                Id = UserProfileDocId,
                Name = Name,
                Email = Email,
                Address = Address, 
                ImageData = ImageData
            };

            // tag::saveUserProfileUsingRepo[]
            bool? success = UserProfileRepository?.Save(userProfile);
            // end::saveUserProfileUsingRepo[]

            if (success.HasValue && success.Value)
            {
                return AlertService.ShowMessage(null, "Successfully updated profile!", "OK");
            }

            return AlertService.ShowMessage(null, "Error updating profile!", "OK");
        }

        async Task SelectImage()
        {
            var imageData = await MediaService.PickPhotoAsync();

            if (imageData != null)
            {
                ImageData = imageData;
            }
        }

        void Logout()
        {
            UserProfileRepository.Dispose();

            AppInstance.User = null;

            LogoutSuccessful?.Invoke();
            LogoutSuccessful = null;
        }
    }
}
