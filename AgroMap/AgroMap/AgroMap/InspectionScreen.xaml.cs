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

        private void InitComponents()
        {
            lbl_inspections.Text = Strings.AvailableInspections;

            if (UserService.LoadUserSession().Level == 0)
                CreateSuperOptions();

            //ListView
            list_view_inspections.ItemTapped += ListView_Tapped;
            list_view_inspections.HasUnevenRows = true;
            list_view_inspections.ItemTemplate = new DataTemplate(() => { return new InspectionCell(this); });            
            Command refreshCommand = new Command(() => LoadInspections());
            list_view_inspections.RefreshCommand = refreshCommand;
            

        }

        private void CreateSuperOptions()
        {
            super_options.Orientation = StackOrientation.Horizontal;
            Button btn_create = new Button();
            btn_create.Text = Strings.NewInspection;
            btn_create.HorizontalOptions = LayoutOptions.FillAndExpand;
            super_options.Children.Add(btn_create);
            btn_create.Clicked += Btn_New_Inspection_Click;
            super_options.IsVisible = true;
        }

        private async void LoadInspections()
        {
            if (isRefreshing)
                return;
            isRefreshing = true;
            if (!(await InspectionService.SyncWithServer()))
            {
                await DisplayAlert(Strings.Warning, Strings.CannotSync + "\n" + Strings.LoadedFromLocal, Strings.OK);
            }
            List<Inspection> __inspections = await InspectionDAO.GetLocalItens();
            list_view_inspections.ItemsSource = __inspections;
            list_view_inspections.IsRefreshing = false;
            isRefreshing = false;
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


        public void ListView_Inspection_Details(Inspection inspection)
        {
            
        }

        public void ListView_Inspection_Members(Inspection inspection)
        {

        }
        public async void ListView_Inspection_Edit(Inspection inspection)
        {
            await Navigation.PushAsync(new NewInspectionScreen(inspection));
        }
        public void ListView_Inspection_Delete(Inspection inspection)
        {

        }

    }
}