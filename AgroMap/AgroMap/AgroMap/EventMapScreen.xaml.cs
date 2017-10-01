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
        private EventTabScreen masterPage;

        public EventMapScreen(EventTabScreen __masterPage)
        {
            InitializeComponent();
            this.masterPage = __masterPage;
        }
    }
}