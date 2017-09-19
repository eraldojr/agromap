using AgroMap.Entity;
using AgroMap.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroMap
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginScreen : ContentPage
    {
        public LoginScreen()
        {
            InitializeComponent();
            InitComponents();
        }

        private void InitComponents()
        {
            img_login.Source = ImageSource.FromFile("@drawable/logofull.png");

        }

        public async void btn_signup_login_Clicked(object sender, EventArgs e)
        {
            try
            {
                Navigation.InsertPageBefore(new SignupScreen(), this);
                actIndLogin.IsRunning = false;
                await Navigation.PopAsync();
                //new NavigationPage(new MainScreen());

            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|LoginScreen.cs|btn_signup_login_Clicked: " + err.Message);
            }

        }

        async void btn_signin_login_Clicked(object sender, EventArgs e)
        {
            actIndLogin.IsVisible = true;
            actIndLogin.IsRunning = true;
            login_itens.IsVisible = false;

            User user = new User()
            {
                Email = txt_email_login.Text,
                Password = txt_pass_login.Text
            };

            Boolean success = await UserService.Signin(user);
            if (success)
            {
                Navigation.InsertPageBefore(new MainScreen(), this);
                actIndLogin.IsRunning = false;
                await Navigation.PopAsync();
            }
            else
            {
                txt_email_login.Text = "Login failed";
                actIndLogin.IsVisible = false;
                actIndLogin.IsRunning = false;
                login_itens.IsVisible = true;
            }
        }
     
        
    }
}