using AgroMap.Database;
using AgroMap.Entity;
using AgroMap.Resources;
using AgroMap.Services;
using AgroMap.Views;
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
    public partial class EventListScreen : ContentPage
    {
        public EventTabScreen __masterPage { get; set; }

        public EventListScreen(EventTabScreen __masterPage)
        {
            this.__masterPage = __masterPage;
            InitializeComponent();
            InitComponents();
            LoadEvents();
        }

        private void InitComponents()
        {
            lbl_events.Text = Strings.Events;
            lbl_inspection_name.Text = this.__masterPage.inspection.name;
            list_view_events.ItemTemplate = new DataTemplate(() => {return new EventCell(this); });
            Command RefreshListCommand = new Command(() => LoadEvents());
            list_view_events.RefreshCommand = RefreshListCommand;
        }

        public async void LoadEvents() 
        {
            try
            {
                list_view_events.ItemsSource = await EventDAO.GetEventsByInspection(this.__masterPage.inspection.id); 
                list_view_events.IsRefreshing = false;
            }
            catch (Exception err)
            {

                Debug.WriteLine("AGROMAP|EventListScreen.cs|LoadEvents: " + err.Message);
            }
            return;
            
        }

        public void ListView_Events_Details(object sender, EventArgs e)
        {
            __masterPage.CurrentPage = __masterPage.Children[2];
        }

        public async void ListView_Events_Edit(Event __event)
        {
            int userID = UserService.GetLoggedUserId();
            if (__event.user == userID || __masterPage.inspection.supervisor == userID)
            {
                __masterPage.Children.Remove(__masterPage.Children[2]);
                __masterPage.new_event_screen = new NewEventScreen(this.__masterPage, __event);
                __masterPage.Children.Add(__masterPage.new_event_screen);
                __masterPage.Children[2].Title = Strings.Edit;
                __masterPage.CurrentPage = __masterPage.Children[2];
                return;
            }
            else
            {
                await DisplayAlert(Strings.Unauthorized, Strings.MustBeCreatorOrSupervisor, Strings.OK);
                return;
            }
           
        }

        public void ListView_Events_ShowOnMap(Event __event)
        {
            __masterPage.ShowEventOnMap(__event);
        }

        public async void ListView_Events_Delete(Event __event)
        {
            int userID = UserService.GetLoggedUserId();
            if(__event.user == userID || __masterPage.inspection.supervisor == userID)
            {
                await EventDAO.Delete(__event.uuid);
                LoadEvents();
                return;
            }
            else
            {
                await DisplayAlert(Strings.Unauthorized, Strings.MustBeCreatorOrSupervisor, Strings.OK);
                return;
            }

        }

        private void list_view_events_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Event __event = e.Item as Event;
        }
    }
}