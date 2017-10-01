using AgroMap.Entity;
using AgroMap.Resources;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroMap
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupScreen : ContentPage
    {

        public SignupScreen()
        {
            InitializeComponent();
            lbl_login.Text = Strings.Signup;
            lbl_name_signup.Text = Strings.Name;
            lbl_last_name_signup.Text = Strings.Last_name;
            lbl_email_signup.Text = Strings.Email;
            lbl_password_signup.Text = Strings.Password;
            btn_back_signup.Text = Strings.Cancel;
            btn_send_signup.Text = Strings.Signup;

        }

        private void btn_back_signup_Clicked(object sender, EventArgs e)
        {
            BackToLogin();
        }

        private async void BackToLogin()
        {
            try
            {
                Navigation.InsertPageBefore(new LoginScreen(), this);
                await Navigation.PopAsync();
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|LoginScreen.cs|btn_back_signup_Clicked: " + err.Message);
            }
        }

        private async void btn_send_signup_Clicked(object sender, EventArgs e)
        {

            int errors = 0;
            if (ent_email_signup.Text.Equals(string.Empty)) errors += 1;
            if (ent_name_signup.Text.Equals(string.Empty)) errors += 1;
            if (ent_last_name_signup.Text.Equals(string.Empty)) errors += 1;
            if (ent_password_signup.Text.Equals(string.Empty)) errors += 1;
            if(errors > 0)
            {
                await DisplayAlert(Strings.Attention, Strings.EmptyFields, Strings.OK);
                return;
            }
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert(Strings.Error, Strings.NoInternet, Strings.OK);
                return;
            }
            ShowAnimation();

            try
            {
                User u = new User
                {
                    Name = ent_name_signup.Text,
                    Last_Name = ent_last_name_signup.Text,
                    Email = ent_email_signup.Text,
                    Password = ent_password_signup.Text
                };
                int response_code = await UserService.Signup(u);
                if (response_code == 201)
                {
                    await DisplayAlert(Strings.Success, Strings.SuccessSignup, Strings.OK);
                    BackToLogin();
                }
                else if (response_code == 400)
                {
                    await DisplayAlert(Strings.Error, Strings.UserDataError, Strings.OK);
                    HideAnimation();
                    return;
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|LoginScreen.cs|btn_send_signup_Clicked: " + err.Message);
            }
            await DisplayAlert(Strings.Error, Strings.UnexpectedError, Strings.OK);
            HideAnimation();
            return;
        }

        private void ShowAnimation()
        {
            actIndLogin.IsVisible = true;
            actIndLogin.IsRunning = true;
            signup_itens.IsVisible = false;
        }

        private void HideAnimation()
        {
            actIndLogin.IsVisible = false;
            actIndLogin.IsRunning = false;
            signup_itens.IsVisible = true;
        }
    }
    
}