using AgroMap.Database;
using AgroMap.Entity;
using AgroMap.Resources;
using AgroMap.Services;
using AgroMap.Views;
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
    public partial class InspectionScreen : ContentPage
    {

        private Boolean isRefreshing = false;
        public InspectionScreen()
        {
            InitializeComponent();
            InitComponents();
            LoadInspections();
        }

        override
        protected async void OnAppearing()
        {
            List<Inspection> __inspections = await InspectionDAO.GetAll();
            list_view_inspections.ItemsSource = __inspections;
        }

        private void InitComponents()
        {
            lbl_inspections.Text = Strings.AvailableInspections;

            if (UserService.LoadUserSession().Level == 0)
                ShowSuperOptions();
            //ListView
            list_view_inspections.ItemTapped += ListView_Tapped;
            list_view_inspections.HasUnevenRows = true;
            list_view_inspections.ItemTemplate = new DataTemplate(() => { return new InspectionCell(this); });            
            Command refreshCommand = new Command(() => LoadInspections());
            list_view_inspections.RefreshCommand = refreshCommand;
        }

        private void ShowSuperOptions()
        {
            btn_create.Text = Strings.NewInspection;
            super_options.IsVisible = true;
        }

        private async void LoadInspections()
        {
            if (isRefreshing)
                return;
            isRefreshing = true;
            ShowAnimation();
            if (!(await InspectionService.SyncWithServer()))
            {
                HideAnimation();
                await DisplayAlert(Strings.Warning, Strings.CannotSync + "\n" + Strings.LoadedFromLocal, Strings.OK);
                return;
            }
            List<Inspection> __inspections = await InspectionDAO.GetAll();
            list_view_inspections.ItemsSource = __inspections;
            list_view_inspections.IsRefreshing = false;
            isRefreshing = false;
            HideAnimation();
        }

        private void HideAnimation()
        {
            actInspectionScreen.IsVisible = false;
            actInspectionScreen.IsRunning = false;
            main_layout.IsVisible = true;
        }

        private void ShowAnimation()
        {
            actInspectionScreen.IsVisible = true;
            actInspectionScreen.IsRunning = true;
            main_layout.IsVisible = false;
        }

        private async void ListView_Members(object sender, ItemTappedEventArgs e)
        {
            Inspection i = e.Item as Inspection;
            await Navigation.PushAsync(new EventTabScreen(i));
        }

        private async void ListView_Tapped(object sender, ItemTappedEventArgs e)
        {
            Inspection i = e.Item as Inspection;
            await Navigation.PushAsync(new EventTabScreen(i));
        }

        private async void ShowNewInspectScreen()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                await Navigation.PushAsync(new NewInspectionScreen());
            }
            else
            {
                await DisplayAlert(Strings.Error, Strings.NoInternet, Strings.OK);
            }
        }

        private async void Btn_New_Inspection_Click(object sender, EventArgs e)
        {
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    ShowNewInspectScreen();
                }
                else
                {
                    await DisplayAlert(Strings.Error, Strings.NoInternet, Strings.OK);
                }
            }catch(Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionScreen|Btn_New_Inspection(): " + err.Message);
            }
        }

        public async void ListView_Inspection_Edit(Inspection inspection)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert(Strings.Error, Strings.NoInternet, Strings.OK);
                return;
            }

            try
            {
                int logged_id = UserService.GetLoggedUserId();
                if (inspection.supervisor == logged_id)
                {
                    await Navigation.PushAsync(new NewInspectionScreen(inspection));
                }
                else
                {
                    await DisplayAlert(Strings.Unauthorized, Strings.MustBeSupervisor, Strings.OK);
                    return;
                }

            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionScreen|ListView_Inspection_Delete:: " + err.Message);
            }
        }

        public async void ListView_Inspection_Delete(Inspection inspection)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert(Strings.Error, Strings.NoInternet, Strings.OK);
                return;
            }
            int logged_id = UserService.GetLoggedUserId();
            if (inspection.supervisor != logged_id)
            {
                await DisplayAlert(Strings.Unauthorized, Strings.MustBeSupervisor, Strings.OK);
                return;
            }
            if ( await InspectionService.DeleteInspection(inspection))
            {
                await DisplayAlert(Strings.Success, Strings.DeletedInspection, Strings.OK);
                return;
            }
            else
            {
                await DisplayAlert(Strings.Error, Strings.UnexpectedError, Strings.OK);
                return;
            }
        }

    }
}