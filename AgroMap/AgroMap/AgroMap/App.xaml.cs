using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AgroMap
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            LoadFirstPage();
        }

        private void LoadFirstPage()
        {
            if (UserService.LoadUserSession() != null)
            {
                MainPage = new NavigationPage(new MainScreen());
            }
            else
            {
                MainPage = new NavigationPage(new LoginScreen());
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            //LoadFirstPage();
        }

 
    }
}
