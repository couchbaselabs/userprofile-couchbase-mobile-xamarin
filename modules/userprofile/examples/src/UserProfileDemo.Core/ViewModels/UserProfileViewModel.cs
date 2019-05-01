using System.Threading.Tasks;
using System.Windows.Input;
using CouchbaseLabs.MVVM;
using CouchbaseLabs.MVVM.Input;
using CouchbaseLabs.MVVM.Services;
using UserProfileDemo.Core.Respositories;
using UserProfileDemo.Core.Services;
using UserProfileDemo.Models;

namespace UserProfileDemo.Core.ViewModels
{
    public class UserProfileViewModel : BaseNavigationViewModel
    {
        IUserProfileRepository UserProfileRepository { get; set; }
        IAlertService AlertService { get; set; }
        IMediaService MediaService { get; set; }

        string UserProfileDocId => $"user::{AppInstance.User.Username}";

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

        string _university;
        public string University
        {
            get => _university ?? "Select University";
            set => SetPropertyChanged(ref _university, value);
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

        ICommand _selectUniversityCommand;
        public ICommand SelectUniversityCommand
        {
            get
            {
                if (_selectUniversityCommand == null)
                {
                    _selectUniversityCommand = new Command(async () => await NavigateToUniversities());
                }

                return _selectUniversityCommand;
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

        public UserProfileViewModel(INavigationService navigationService, 
                                    IUserProfileRepository userProfileRepository,
                                    IAlertService alertService,
                                    IMediaService mediaService) : base(navigationService)
        {
            UserProfileRepository = userProfileRepository;

            AlertService = alertService;
            MediaService = mediaService;
        }

        public override async Task InitAsync()
        {
            IsBusy = true;

            await UserProfileRepository.StartReplicationForCurrentUser().ConfigureAwait(false);

            var userProfile = await Task.Run(async () =>
            {
                var up = await UserProfileRepository?.GetAsync(UserProfileDocId, UpdateUserProfile);

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
                UpdateUserProfile(userProfile);
            }

            IsBusy = false;
        }

        void UpdateUserProfile(UserProfile userProfile)
        {
            Name = userProfile.Name;
            Email = userProfile.Email;
            Address = userProfile.Address;
            ImageData = userProfile.ImageData;
            University = userProfile.University;
        }

        async Task Save()
        {
            var userProfile = new UserProfile
            {
                Id = UserProfileDocId,
                Name = Name,
                Email = Email,
                Address = Address, 
                ImageData = ImageData,  
                University = University
            };
   
            var success = await UserProfileRepository.SaveAsync(userProfile).ConfigureAwait(false);

            if (success)
            {
                await AlertService.ShowMessage(null, "Successfully updated profile!", "OK");
            }
            else
            {
                await AlertService.ShowMessage(null, "Error updating profile!", "OK");
            }
        }

        async Task SelectImage()
        {
            var imageData = await MediaService.PickPhotoAsync();

            if (imageData != null)
            {
                ImageData = imageData;
            }
        }

        Task NavigateToUniversities()
        {
            var vm = ServiceContainer.GetInstance<UniversitiesViewModel>();

            vm.UniversitySelected = UniversitySelected;

            return Navigation.PushAsync(vm);
        }

        void UniversitySelected(string name) => University = name;

        void Logout()
        {
            UserProfileRepository.Dispose();

            AppInstance.User = null;

            Navigation.ReplaceRoot<LoginViewModel>(false);
        }
    }
}
