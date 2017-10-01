using AgroMap.Entity;
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
        public Inspection inspection;

        public EventTabScreen(Inspection inspection)
        {
            InitializeComponent();
            this.inspection = inspection;
            InitComponents();
        }

        private void InitComponents()
        {
            event_list_content_page.Content = new EventListScreen(this).Content;
            event_map_content_page.Content = new EventMapScreen(this).Content;
            event_new_content_page.Content = new NewEventScreen(this).Content;
        }
    }
}