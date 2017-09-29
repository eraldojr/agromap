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
        public EventTabScreen masterPage { get; set; }

        public EventListScreen(EventTabScreen masterPage)
        {
            this.masterPage = masterPage;
            InitializeComponent();
            InitComponents();
            LoadEvents();

        }

        private void InitComponents()
        {
            lbl_events.Text = Strings.Events;

            lbl_id.Text = this.masterPage.inspection.id.ToString();
            lbl_inspection_id.Text = Strings.Inspection + " " + Strings.ID;
            lbl_inspection_name.Text = this.masterPage.inspection.name;


            list_view_events.ItemTemplate = new DataTemplate(() => {return new EventCell(this); });
            Command RefreshListCommand = new Command(() => LoadEvents());
            list_view_events.RefreshCommand = RefreshListCommand;
        }

        private async void LoadEvents() 
        {
            list_view_events.ItemsSource = await EventDAO.GetEventsByInspection(this.masterPage.inspection.id);
            return;
            
        }

        public void ListView_Events_Details(object sender, EventArgs e)
        {
            masterPage.CurrentPage = masterPage.Children[2];
        }

        public void ListView_Events_Delete(object sender, EventArgs e)
        {

        }
        public async void ListView_Events_Edit(object sender, EventArgs e)
        {
            
        }
        public void ListView_Events_ShowOnMap(object sender, EventArgs e)
        {

        }
    }
}