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
    public partial class EventMapScreen : ContentPage
    {
        private EventTabScreen __masterPage;
        private Event __event;

        public EventMapScreen(EventTabScreen __masterPage)
        {
            InitializeComponent();
            this.__masterPage = __masterPage;
        }
        public EventMapScreen(EventTabScreen __masterPage, Event __event)
        {
            InitializeComponent();
            this.__masterPage = __masterPage;
            this.__event = __event;
        }
    }
}