using AgroMap.Database;
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
    public partial class NewEventScreen : ContentPage
    {
        private Event __event;
        private EventTabScreen __masterPage;

        public NewEventScreen(EventTabScreen __masterPage)
        {
            InitializeComponent();
            InitComponents();
            this.__masterPage = __masterPage;
            this.__event = null;
        }

        public NewEventScreen(EventTabScreen __masterPage, Event __event)
        {
            InitializeComponent();
            InitComponents();
            this.__masterPage = __masterPage;
            this.__event = __event;
            SetEditableEvent();
        }

        private void InitComponents()
        { 
            lbl_main.Text = Strings.New + " " + Strings.Event;

            lbl_kind.Text = Strings.Typeof;
            lbl_description.Text = Strings.Description;
            lbl_latitude.Text = Strings.Latitude;
            lbl_longitude.Text = Strings.Longitude;

            pck_kind.Items.Add(Strings.Checked);
            pck_kind.Items.Add(Strings.Problem);
            pck_kind.Items.Add(Strings.Observation);
            pck_kind.SelectedIndex = 0;

            btn_save.Text = Strings.Save;
            btn_cancel.Text = Strings.Cancel;

        }


        public void SetEditableEvent()
        {
            if (this.__event == null)
                return;
            lbl_main.Text = Strings.Edit + " " + Strings.Event;
            ent_description.Text = __event.description;
            ent_longitude.Text = __event.longitude;
            ent_latitude.Text = __event.latitude;
        }

        private async void Btn_save_Clicked(object sender, EventArgs e)
        {
            Boolean result = false;
            try
            {
                if (__event != null)
                {
                    __event.description = ent_description.Text;
                    __event.kind = __event.kind;
                    __event.description = ent_description.Text;
                    __event.last_edit_at = DateTime.Now;
                    __event.latitude = "41ºSE 51ºLF";
                    __event.longitude = "54ºSW 85ºNW";
                    __event.synced = 0;
                }
                else
                {
                    this.__event = new Event
                    {
                        uuid = "",
                        inspection = this.__masterPage.inspection.id,
                        user = UserService.GetLoggedUserId(),
                        last_edit_at = DateTime.Now,
                        kind = pck_kind.SelectedItem.ToString(),
                        description = ent_description.Text,
                        latitude = "41ºSE 51ºLF",
                        longitude = "54ºSW 85ºNW",
                    };
                }
                result = await EventDAO.Create(this.__event);
                
                if (result)
                {
                    await DisplayAlert(Strings.Success, Strings.CreatedWithSuccess, Strings.OK);
                    BackToList();
                }
                else
                {
                    await DisplayAlert(Strings.Error, Strings.UnexpectedError, Strings.OK);
                    return;
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|NewEventScreen.cs|Btn_save():: " + err);
            }
        }

        private void BackToList()
        {
            this.__event = null;
            this.Title = Strings.New;
            this.ent_description.Text = "";
            this.ent_latitude.Text = "";
            this.ent_longitude.Text = "";
            this.pck_kind.SelectedIndex = 0;
            __masterPage.CurrentPage = __masterPage.Children[0];
        }

        private void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            BackToList();
        }

    }
}