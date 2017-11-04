using AgroMap.Database;
using AgroMap.Resources;
using AgroMap.Services;
using Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroMap
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainScreenDetail : ContentPage
    {
        public MainScreenDetail()
        {
            InitializeComponent();
            welcome.Text = Strings.Welcome;
            txt_welcome.Text = Strings.WelcomeText;
            img_welcome.Source = ImageSource.FromFile("@drawable/logofull.png");
        }

    }
}