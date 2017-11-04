using AgroMap.Database;
using AgroMap.Entity;
using AgroMap.Resources;
using AgroMap.Services;
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
    public partial class EventTabScreen : TabbedPage
    {
        public Inspection inspection { get; set; }
        private EventListScreen event_list_page;
        public NewEventScreen new_event_screen;
        private EventMapScreen event_map_screen;
        public List<Event> events { get; set; } = new List<Event>();
        public Geolocator.Plugin.Abstractions.Position position { get; set; }
        private Event __event;
        private bool isChanging;
        private bool EventOnMap;
        public MainScreenMaster __menu;

        public EventTabScreen(Inspection inspection)
        {
            InitializeComponent();
            this.inspection = inspection;
            GetLocation();
            event_list_page = new EventListScreen(this);
            new_event_screen = new NewEventScreen(this);
            event_map_screen = new EventMapScreen(this);
            
            InitComponents();
            this.CurrentPageChanged += EventTabScreen_CurrentPageChanged;
        }

        private async void GetEvents()
        {
            this.events = await EventDAO.GetEventsByInspection(this.inspection.id);
            return;
        }

        // Ao mudar de aba
        private void EventTabScreen_CurrentPageChanged(object sender, EventArgs e)
        {
            if (isChanging)
                return;
            isChanging = true;
            if (this.CurrentPage == this.Children[0])
            {
                var page = this.Children[0] as EventListScreen;
                event_list_page.LoadEvents();
            }
            else if (this.CurrentPage == this.Children[1])
            {
                
                this.Children.Remove(this.Children[1]);
                this.Children.Remove(this.Children[1]);
                if (this.EventOnMap)
                {
                    event_map_screen = new EventMapScreen(this, this.__event);
                    EventOnMap = false;
                }
                else
                {
                    event_map_screen = new EventMapScreen(this);
                }
                new_event_screen = new NewEventScreen(this);
                this.Children.Add(event_map_screen);
                this.Children.Add(new_event_screen);
                this.Children[1].Title = Strings.Map;
                this.Children[2].Title = Strings.New;
                this.CurrentPage = this.Children[1];
                
            }
            else if(this.CurrentPage == this.Children[2])
            {
                new_event_screen.GetLocation();
            }
            isChanging = false;
        }

        private void InitComponents()
        {
            event_list_content_page.Content = event_list_page.Content;
            event_list_content_page.Title = Strings.Events;
            event_map_content_page.Content = event_map_screen.Content;
            event_map_content_page.Title = Strings.Map;
            event_new_content_page.Content = new_event_screen.Content;
            event_new_content_page.Title = Strings.New;
        }

        public async void GetLocation()
        {
            try
            {
                position = await Geolocator.Plugin.CrossGeolocator.Current.GetPositionAsync();
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventTabScreen.cs|GetLocation: " + err.Message);
            }
        }

        public void ShowEventOnMap(Event __event)
        {
            EventOnMap = true;
            this.__event = __event;
            this.CurrentPage = this.Children[1];
        }
    }
}