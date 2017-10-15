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
        private string latitude = "";
        private string longitude = "";

        public NewEventScreen(EventTabScreen __masterPage)
        {
            InitializeComponent();
            img_checked.Source = ImageSource.FromFile("@drawable/checked.png");
            InitComponents();
            this.__masterPage = __masterPage;
            this.__event = null;    
        }

        public NewEventScreen(EventTabScreen __masterPage, Event __event)
        {
            InitializeComponent();
            img_checked.Source = ImageSource.FromFile("@drawable/checked.png");
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
            lbl_location.Text = Strings.SearchingLocation;

            pck_kind.Items.Add(Strings.Checked);
            pck_kind.Items.Add(Strings.Problem);
            pck_kind.Items.Add(Strings.Observation);
            pck_kind.SelectedIndex = 0;

            btn_save.Text = Strings.Save;
            btn_cancel.Text = Strings.Cancel;
            
        }

        public async void GetLocation()
        {
            img_checked.IsVisible = false;
            actInd_Location.IsVisible = true;
            lbl_location.Text = Strings.SearchingLocation;
            if (this.__event != null)
            {
                SetLocationFound();
                return;
            }
            try
            {
                var position = await Geolocator.Plugin.CrossGeolocator.Current.GetPositionAsync();
                latitude = position.Latitude.ToString();
                longitude = position.Longitude.ToString();
                SetLocationFound();
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|NewEventScreen.cs|GetLocation: " + err.Message);
            }

        }

        private void SetLocationFound()
        {
            actInd_Location.IsVisible = false;
            img_checked.IsVisible = true;
            lbl_location.Text = Strings.LocationFound;
            
        }

        public void SetEditableEvent()
        {
            if (this.__event == null)
                return;
            lbl_main.Text = Strings.Edit + " " + Strings.Event;
            ent_description.Text = __event.description;
        }

        private async void Btn_save_Clicked(object sender, EventArgs e)
        {
            Boolean result = false;
            try
            {
                if (latitude.Equals("") || longitude.Equals(""))
                {
                    await DisplayAlert(Strings.Warning, Strings.LocationNotFound, Strings.OK);
                    return;
                }
            }
            catch(Exception err)
            {
                await DisplayAlert(Strings.Warning, Strings.LocationNotFound, Strings.OK);
                return;
            }
            

            try
            {
                if (__event != null)
                {
                    __event.description = ent_description.Text;
                    __event.kind = __event.kind;
                    __event.description = ent_description.Text;
                    __event.last_edit_at = DateTime.Now;
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
                        latitude = latitude,
                        longitude = longitude
                    };
                    if (this.__event.description == null || this.__event.description.Equals(""))
                        this.__event.description = String.Empty;

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
            latitude = "";
            longitude = "";
            this.pck_kind.SelectedIndex = 0;
            __masterPage.CurrentPage = __masterPage.Children[0];
        }

        private void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            BackToList();
        }

    }
}