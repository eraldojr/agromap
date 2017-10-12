using AgroMap.Entity;
using AgroMap.Resources;
using System;
using System.Collections.Generic;
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

        public EventTabScreen(Inspection inspection)
        {
            InitializeComponent();
            this.inspection = inspection;
            event_list_page = new EventListScreen(this);
            InitComponents();
            this.CurrentPageChanged += EventTabScreen_CurrentPageChanged;
        }

        private void EventTabScreen_CurrentPageChanged(object sender, EventArgs e)
        {
            if(this.CurrentPage == this.Children[0])
            {
                var page = this.Children[0] as EventListScreen;
                event_list_page.LoadEvents();
            }
        }

        private void InitComponents()
        {
            event_list_content_page.Content = event_list_page.Content;
            event_list_content_page.Title = Strings.Events;
            event_map_content_page.Content = new EventMapScreen(this).Content;
            event_map_content_page.Title = Strings.Map;
            event_new_content_page.Content = new NewEventScreen(this).Content;
            event_new_content_page.Title = Strings.New;
        }
    }
}