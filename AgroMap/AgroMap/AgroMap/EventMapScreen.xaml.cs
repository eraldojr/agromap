using AgroMap.Database;
using AgroMap.Entity;
using AgroMap.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace AgroMap
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventMapScreen : ContentPage
    {
        private EventTabScreen __masterPage;
        private Event __event;
        public List<Event> events { get; set; }
        public List<CustomPin> customPins { get; set; }
        public List<Pin> list_pins { get; set; }
        
        private CustomMap customMap;

        public EventMapScreen(EventTabScreen __masterPage)
        {
            InitializeComponent();
            this.__masterPage = __masterPage;

            customMap = new CustomMap
            {
                MapType = MapType.Street,
            };
            
            events = GetEvents();

            List<CustomPin> cPins = new List<CustomPin>();
            List<Pin> pins = new List<Pin>();
         
            foreach (Event e in events)
            {
                CustomPin pin = new CustomPin
                {
                    Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(Convert.ToDouble(e.latitude), Convert.ToDouble(e.longitude)),
                        Label = e.kind,
                    },
                    Id = e.uuid
                };
                cPins.Add(pin);
                pins.Add(pin.Pin);
            }
            customMap.CustomPins = cPins;
            foreach (Pin p in pins)
            {
                customMap.Pins.Add(p);
            }
           
            var position = __masterPage.position;
            if(position != null)
            {
                customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMiles(1.0)));
                CustomPin pin = new CustomPin
                {
                    Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(position.Latitude, position.Longitude),
                        Label = Strings.You,
                    },
                };
                customMap.CustomPins.Add(pin);
                customMap.Pins.Add(pin.Pin);
            }
            else
            {
                customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-22.7146204, -47.6918727), Distance.FromMiles(15.0)));
            }

            Content = customMap;
        }

        private List<Event> GetEvents()
        {
            try
            {
                List<Event> _aEvents = new List<Event>();
                var task = Task.Run(async () => { _aEvents = await EventDAO.GetEventsByInspection(__masterPage.inspection.id); });
                task.Wait();
                return _aEvents;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventMapScreen.cs|GetEvents: " + err.Message);
                return new List<Event>();
            }
        }

        public EventMapScreen(EventTabScreen __masterPage, Event __event)
        {
            InitializeComponent();
            this.__masterPage = __masterPage;
            this.__event = __event;

            customMap = new CustomMap
            {
                MapType = MapType.Street,
            };

            events = GetEvents();

            List<CustomPin> cPins = new List<CustomPin>();
            List<Pin> pins = new List<Pin>();

            foreach (Event e in events)
            {
                CustomPin pin = new CustomPin
                {
                    Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(Convert.ToDouble(e.latitude), Convert.ToDouble(e.longitude)),
                        Label = e.kind,
                    },
                    Id = e.uuid
                };
                cPins.Add(pin);
                pins.Add(pin.Pin);
            }
            customMap.CustomPins = cPins;
            foreach (Pin p in pins)
            {
                customMap.Pins.Add(p);
            }

            var position = new Position(Convert.ToDouble(__event.latitude), Convert.ToDouble(__event.longitude));
            if (position != null)
            {
                customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMiles(0.1)));
                if (__masterPage.position != null)
                {
                    CustomPin pin = new CustomPin
                    {
                        Pin = new Pin
                        {
                            Type = PinType.Generic,
                            Position = new Position(__masterPage.position.Latitude, __masterPage.position.Longitude),
                            Label = Strings.You,
                        },
                    };
                    customMap.CustomPins.Add(pin);
                    customMap.Pins.Add(pin.Pin);
                }
            }
            else
            {
                customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-22.7146204, -47.6918727), Distance.FromMiles(15.0)));
            }

            Content = customMap;
        }
        
     

        
    }
}