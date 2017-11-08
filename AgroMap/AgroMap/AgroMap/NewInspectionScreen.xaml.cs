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

            lbl_start.Text = Strings.StartOn;

            lbl_end.Text = Strings.EndsOn;

            ent_name.TextChanged += Ent_name_TextChanged;

            btn_save.Text = Strings.Save;
            btn_save.Clicked += Btn_save_Clicked;

            btn_cancel.Text = Strings.Cancel;
            btn_cancel.Clicked += Btn_cancel_Clicked;


            if (this.inspection != null)
            {
                LoadValues();
            }
        }

        private void Ent_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(ent_name.Text.Length > 40)
            {
                var text = ent_name.Text;
                text = text.Remove(text.Length - 1);
                ent_name.Text = text;
            }
        }

        private async void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        

        private async void Btn_save_Clicked(object sender, EventArgs e)
        {
            if (!VerifyFields())
            {
                await DisplayAlert(Strings.Error, Strings.VerifyFieldsInspection, Strings.OK);
                return;
            }
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert(Strings.Error, Strings.ToManageInspection, Strings.OK);
                return;
            }
            ShowAnimation();
            var s = DateTime.SpecifyKind(pck_end.Date, DateTimeKind.Utc).ToLocalTime();
            string x = s.Month.ToString() + "/" + (s.Day + 1).ToString() + "/" + s.Year.ToString() + " 11:59:59 PM";
            var end_date = Convert.ToDateTime(x);

            if (inspection != null)
            {
                inspection.name = ent_name.Text;
                inspection.start_at = DateTime.SpecifyKind(pck_start.Date, DateTimeKind.Utc).ToLocalTime();
                inspection.end_at = end_date;
            }
            else
            {
                this.inspection = new Inspection
                {
                    id = 0,
                    name = ent_name.Text,
                    start_at = DateTime.SpecifyKind(pck_start.Date, DateTimeKind.Utc).ToLocalTime(),
                    end_at = end_date
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
                HideAnimation();
                return;
            }
        }

        private bool VerifyFields()
        {
            int date_start = pck_start.Date.Year * 10000 + pck_start.Date.Month * 100 + pck_start.Date.Day;
            int date_end = pck_end.Date.Year * 10000 + pck_end.Date.Month * 100 + pck_end.Date.Day;
            int date_today = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;

            int errors = 0;
            try
            {
                if (ent_name.Text.Length < 3)
                    errors += 1;
                if (date_start < date_today)
                    errors += 1;
                if (date_end < date_start)
                    errors += 1;
                if (errors > 0)
                    return false;
                return true;
            }catch(Exception err)
            {
                Debug.WriteLine("AGROMAP|NewInspectionScreens.cs|VerifyFields: " + err.Message);
                return false;
            }

        }

        private void LoadValues()
        {
            lbl_main.Text = Strings.EditInspection;
            ent_name.Text = this.inspection.name;
            pck_start.Date = this.inspection.start_at;
            pck_end.Date = this.inspection.end_at;

        }


        private void HideAnimation()
        {
            actInspectionScreen.IsVisible = false;
            actInspectionScreen.IsRunning = false;
            layout_itens.IsVisible = true;
        }

        private void ShowAnimation()
        {
            actInspectionScreen.IsVisible = true;
            actInspectionScreen.IsRunning = true;
            layout_itens.IsVisible = false;
        }

    }
}