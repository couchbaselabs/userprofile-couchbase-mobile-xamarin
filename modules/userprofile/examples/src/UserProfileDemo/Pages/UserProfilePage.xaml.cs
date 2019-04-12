using System;
using CouchbaseLabs.MVVM.Forms.Pages;
using UserProfileDemo.Core.ViewModels;

namespace UserProfileDemo.Pages
{
    public partial class UserProfilePage : BaseContentPage<UserProfileViewModel>
    {
        public UserProfilePage()
        {
            InitializeComponent();
        }
    }
}
