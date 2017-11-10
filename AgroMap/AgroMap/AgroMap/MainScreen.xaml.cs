using AgroMap.Services;
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
    public partial class MainScreen : MasterDetailPage
    {
        public MainScreen()
        {
            InitializeComponent();
            __masterPage.ListView.ItemSelected += ListView_ItemSelected;
            __masterPage.Appearing += __masterPage_Appearing;

            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void __masterPage_Appearing(object sender, EventArgs e)
        {
            
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            ((ListView)sender).SelectedItem = null;

            var item = e.SelectedItem as MainScreenMenuItem;
            if (item == null)
                return;

            if (item.Id == 0)
            {
                __masterPage.UpdateSyncLabel();
                await Navigation.PushAsync(new InspectionScreen(__masterPage)); 
            }
            else if (item.Id == 1)
            {
                await InspectionService.SyncWithServer();
                __masterPage.UpdateSyncLabel();
            }
            else if (item.Id == 2)
            {
                UserService.Logout();
                Navigation.InsertPageBefore(new LoginScreen(), this);
                await Navigation.PopAsync();
                Navigation.RemovePage(this);
            }

        }
    }
}