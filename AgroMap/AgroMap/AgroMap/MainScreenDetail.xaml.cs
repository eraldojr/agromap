using AgroMap.Database;
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
        }

        private void btn_upload_Clicked(object sender, EventArgs e)
        {
            try
            {
                DependencyService.Get<IFilePicker>().DeleteDirectory();
                EventDAO.DeleteFromInspection(1);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}