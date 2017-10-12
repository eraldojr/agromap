using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroMap.iOS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IOSDevice : ContentPage
    {
        public IOSDevice()
        {
            InitializeComponent();
        }
    }
}