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
        private EventTabScreen masterPage;

        public NewEventScreen(EventTabScreen __masterpage)
        {
            InitializeComponent();
            InitComponents();
            this.__event = null;
            this.masterPage = __masterpage;
        }

        public NewEventScreen(Event __event)
        {
            InitializeComponent();
            InitComponents();
            this.__event = __event;
        }

        private void InitComponents()
        { 
            //Atribuindo propriedades
            if (__event != null)
            {
                lbl_main.Text = Strings.Edit + " " + Strings.Event;
                ent_id.Text = __event.id.ToString();
            }
            else
            {
                lbl_main.Text = Strings.New + " " + Strings.Event;
            }

            ent_id.IsEnabled = false;

            lbl_id.Text = Strings.ID;
            lbl_type.Text = Strings.Typeof;
            lbl_description.Text = Strings.Description;


            btn_save.Text = Strings.Save;
            btn_save.Clicked += Btn_save_Clicked;
            btn_cancel.Text = Strings.Cancel;

        }

        private async void Btn_save_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (__event != null)
                {
                    __event.description = ent_description.Text;
                    __event.types = __event.types;
                    __event.description = ent_description.Text;
                    __event.last_edit_at = DateTime.Now;
                    __event.latitude = "41ºSE 51ºLF";
                    __event.longitude = "54ºSW 85ºNW";
                }
                else
                {
                    this.__event = new Event
                    {
                        id = 0,
                        inspection = this.masterPage.inspection.id,
                        user = UserService.GetLoggedUserId(),
                        created_at = DateTime.Now,
                        last_edit_at = DateTime.Now,
                        types = ent_type.Text,
                        description = ent_description.Text,
                        latitude = "41ºSE 51ºLF",
                        longitude = "54ºSW 85ºNW",
                    };
                }

                Boolean result = await EventDAO.Create(this.__event);
                if (result)
                {
                    await DisplayAlert(Strings.Success, Strings.CreatedWithSuccess, Strings.OK);
                   
                    masterPage.CurrentPage = masterPage.Children[0];

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

        private async void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}