using AgroMap.Database;
using AgroMap.Entity;
using AgroMap.Resources;
using AgroMap.Services;
using Plugin.Connectivity;
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
            btn_signin.Text = Strings.Signin;
            btn_signup.Text = Strings.Signup;
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
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert(Strings.Error, Strings.NoInternet, Strings.OK);
                return;
            }
            if (txt_email_login.Text.Equals(String.Empty) || txt_pass_login.Text.Equals(String.Empty))
            {
                await DisplayAlert(Strings.Error, Strings.EmptyFields, Strings.OK);
                return;
            }
            ShowAnimation();

            User user = new User()
            {
                Email = txt_email_login.Text,
                Password = txt_pass_login.Text
            };

            int responseCode = await UserService.Signin(user);
            if (responseCode == 200)
            {
                if (!await InspectionService.SetDeviceUUID())
                {
                    await DisplayAlert(Strings.Error, Strings.ErrorDeviceUUID, Strings.OK);
                    UserService.Logout();
                    HideAnimation();
                    return;
                }
                Navigation.InsertPageBefore(new MainScreen(), this);
                actIndLogin.IsRunning = false;
                await Navigation.PopAsync();
            }
            else if( responseCode == 401)
            {
                HideAnimation();
                await DisplayAlert(Strings.Error, Strings.CredentialsError, Strings.OK);
                return;
            }
            else
            {
                HideAnimation();
                await DisplayAlert(Strings.Error, Strings.UnexpectedError, Strings.OK);
                return;
            }
        }

        private void ShowAnimation()
        {
            actIndLogin.IsVisible = true;
            actIndLogin.IsRunning = true;
            login_itens.IsVisible = false;
        }

        private void HideAnimation()
        {
            actIndLogin.IsVisible = false;
            actIndLogin.IsRunning = false;
            login_itens.IsVisible = true;
        }
    }
}