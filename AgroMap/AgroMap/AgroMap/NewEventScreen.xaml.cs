using AgroMap.Database;
using AgroMap.Entity;
using AgroMap.Resources;
using System;
using Xamarin.Forms;
using Android.Content.PM;
using System.Diagnostics;
using Xamarin.Forms.Xaml;
using AgroMap.Services;
using Android;
using System.Threading.Tasks;
using System.IO;

namespace AgroMap
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewEventScreen : ContentPage
    {
        private Event __event;
        private EventTabScreen __masterPage;
        private string latitude = "";
        private string longitude = "";
        private static string coarseLocation = Manifest.Permission.AccessCoarseLocation;
        private static string fineLocation = Manifest.Permission.AccessFineLocation;
        private static string storage_write = Manifest.Permission.WriteExternalStorage;
        private static string storage_read = Manifest.Permission.ReadExternalStorage;
        private Image event_image;
        private bool hasPhoto;

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

            btn_gallery.Text = Strings.InsertPhoto;

        }

        public async void GetLocation()
        {
            if (!CheckLocationPermission())
                BackToList();

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

        private static Boolean CheckLocationPermission()
        {
            try
            {
                DependencyService.Get<IGetPermission>().CheckPermission(fineLocation);
                DependencyService.Get<IGetPermission>().CheckPermission(coarseLocation);
                return true;

            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|NewEventScreen.cs|CheckLocationPermission: " + err.Message);
            }
            return false;
            
        }
        
        private static Boolean CheckStoragePermission()
        {
            try
            {
                DependencyService.Get<IGetPermission>().CheckPermission(storage_read);
                DependencyService.Get<IGetPermission>().CheckPermission(storage_write);
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|NewEventScreen.cs|CheckStoragePermission: " + err.Message);
            }
            return false;

        }

        private void SetLocationFound()
        {
            actInd_Location.IsVisible = false;
            img_checked.IsVisible = true;
            lbl_location.Text = Strings.LocationFound;
            
        }

        public void SetEditableEvent()
        {
            if (__event == null)
                return;
            lbl_main.Text = Strings.Edit + " " + Strings.Event;
            ent_description.Text = __event.description;
            latitude = __event.latitude;
            longitude = __event.longitude;
            RetrievePhoto(__event.uuid);

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
            catch(Exception)
            {
                await DisplayAlert(Strings.Warning, Strings.LocationNotFound, Strings.OK);
                return;
            }

            try
            {
                var s = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).ToLocalTime();
                string x = s.Month.ToString() + "/" + (s.Day + 1).ToString() + "/" + s.Year.ToString() + " " + DateTime.Now.TimeOfDay;
                var current_date = Convert.ToDateTime(x);
                if (__event != null)
                {
                    __event.description = ent_description.Text;
                    __event.kind = __event.kind;
                    __event.description = ent_description.Text;
                    __event.last_edit_at = current_date;
                }
                else
                {
                    this.__event = new Event
                    {
                        uuid = "",
                        inspection = this.__masterPage.inspection.id,
                        user = UserService.GetLoggedUserId(),
                        last_edit_at = current_date,
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

        private void RetrievePhoto(string uuid)
        {
            ImageSource imgSrc = PhotoService.RetrievePhoto(uuid);

            if (imgSrc != null)
            {
                event_image = new Image
                {
                    //Source = ImageSource.FromStream(() => stream),
                    Source = imgSrc,
                    BackgroundColor = Color.Gray
                };

                imageView.Source = event_image.Source;
            }
        }

        private void BackToList()
        {
            __event = null;
            imageView.Source = null;
            Title = Strings.New;
            ent_description.Text = "";
            event_image = null;
            latitude = "";
            longitude = "";
            pck_kind.SelectedIndex = 0;
            __masterPage.CurrentPage = __masterPage.Children[0];
        }

        private void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            imageView.Source = null;
            event_image = null;
            
            var photo_uuid = InspectionService.GetNextID();
            if (__event == null && hasPhoto)
            {
                PhotoService.DeleteFile(photo_uuid);
            }
            hasPhoto = false;
            BackToList();
        }

        private async void btn_gallery_Clicked(object sender, EventArgs e)
        {
            hasPhoto = true;
            string uuid = null;
            if (__event != null)
                uuid = __event.uuid;
            ImageSource imgSrc = await PhotoService.AddPhoto(uuid);
            imageView.Source = imgSrc;
        }

    }
}