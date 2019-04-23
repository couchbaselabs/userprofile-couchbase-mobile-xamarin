using CouchbaseLabs.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UserProfileDemo.Repositories.Services;
using UserProfileDemo.UWP.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UserProfileDemo.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage 
    {
        public MainPage()
        {
            this.InitializeComponent();

            // tag::activate[]
            Couchbase.Lite.Support.UWP.Activate();
            // end::activate[]

            RegisterServices();

            LoadApplication(new UserProfileDemo.App());
        }

        void RegisterServices()
        {
            ServiceContainer.Register<IDatabaseSeedService>(() => new DatabaseSeedService());
        }
    }
}
