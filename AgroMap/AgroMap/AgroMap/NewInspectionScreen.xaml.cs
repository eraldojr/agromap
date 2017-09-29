using AgroMap.Entity;
using AgroMap.Resources;
using AgroMap.Services;
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
    public partial class NewInspectionScreen : ContentPage
    {
        private Inspection inspection = null;
       
        public NewInspectionScreen()
        {
            InitializeComponent();
            InitComponents();
        }

        public NewInspectionScreen(Inspection inspection)
        {
            InitializeComponent();
            this.inspection = inspection;
            InitComponents();
        }

        private void InitComponents()
        {

            lbl_main.Text = Strings.NewInspection;

            lbl_name.Text = Strings.Name;

            lbl_start.Text = Strings.StartAt;

            lbl_end.Text = Strings.EndAt;

            btn_save.Text = Strings.Save;
            btn_save.Clicked += Btn_save_Clicked;

            btn_cancel.Text = Strings.Cancel;
            btn_cancel.Clicked += Btn_cancel_Clicked;


            if (this.inspection != null)
            {
                LoadValues();
            }
        }

        private async void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void Btn_save_Clicked(object sender, EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert(Strings.Error, Strings.ToManageInspection, Strings.OK);
                return;
            }
            if (inspection != null)
            {
                inspection.name = ent_name.Text;
                inspection.start_at = pck_start.Date;
                inspection.end_at = pck_end.Date;
            }
            else
            {
                this.inspection = new Inspection
                {
                    id = 0,
                    name = ent_name.Text,
                    start_at = pck_start.Date,
                    end_at = pck_end.Date
                };
            }
            
            Boolean result = await InspectionService.CreateInspection(this.inspection);
            if (result)
            {
                await DisplayAlert(Strings.Success, Strings.CreatedWithSuccess, Strings.OK);
                await Navigation.PopAsync();

            }
            else
            {
                await DisplayAlert(Strings.Error, Strings.UnexpectedError, Strings.OK);
                return;
            }
        }

        private void LoadValues()
        {
            lbl_main.Text = Strings.EditInspection;
            ent_name.Text = this.inspection.name;
            pck_start.Date = this.inspection.start_at;
            pck_end.Date = this.inspection.end_at;

        }
    }
}