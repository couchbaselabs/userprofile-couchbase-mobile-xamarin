using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CouchbaseLabs.MVVM.Input;
using CouchbaseLabs.MVVM.Services;
using UserProfileDemo.Core.Respositories;
using UserProfileDemo.Models;

namespace UserProfileDemo.Core.ViewModels
{
    public class UniversitiesViewModel : BaseNavigationViewModel
    {
        public Action<string> UniversitySelected { get; set; }

        IUniversityRepository UniversityRepository { get; set; }

        string _name;
        public string Name
        {
            get => _name;
            set
            {
                SetPropertyChanged(ref _name, value);
                SetPropertyChanged(nameof(IsSearchEnabled));
            }
        }

        string _country;
        public string Country
        {
            get => _country;
            set =>SetPropertyChanged(ref _country, value);
        }

        public bool IsSearchEnabled
        {
            get => !string.IsNullOrEmpty(Name);
        }

        List<University> _universities;
        public List<University> Universities
        {
            get => _universities;
            set => SetPropertyChanged(ref _universities, value);
        }

        ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new Command(async () => await SearchAsync());
                }

                return _searchCommand;
            }
        }

        ICommand _selectCommand;
        public ICommand SelectCommand
        {
            get
            {
                if (_selectCommand == null)
                {
                    _selectCommand = new Command<University>(async (university) => await SelectUniversity(university));
                }

                return _selectCommand;
            }
        }

        public UniversitiesViewModel(INavigationService navigationService,
                                     IUniversityRepository universityRepository) : base(navigationService)
        {
            UniversityRepository = universityRepository;
        }

        Task SelectUniversity(University university)
        {
            UniversitySelected?.Invoke(university.Name);
            return Dismiss();
        }

        async Task SearchAsync()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                Universities = await UniversityRepository.SearchByName(Name, Country);
            }
        }
    }
}
