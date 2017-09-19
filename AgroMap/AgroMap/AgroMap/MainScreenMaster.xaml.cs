using AgroMap.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroMap
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainScreenMaster : ContentPage
    {
        public ListView ListView;

        public MainScreenMaster()
        {
            InitializeComponent();

            BindingContext = new MainScreenMasterViewModel();
            ListView = MenuItemsListView;
        }

        class MainScreenMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MainScreenMenuItem> MenuItems { get; set; }

            public MainScreenMasterViewModel()
            {
                MenuItems = new ObservableCollection<MainScreenMenuItem>(new[]
                {
                    new MainScreenMenuItem { Id = 0, Title = Strings.Inspection },
                    new MainScreenMenuItem { Id = 1, Title = Strings.Logout },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}