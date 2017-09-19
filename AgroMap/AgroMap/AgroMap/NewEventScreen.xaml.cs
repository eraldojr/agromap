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
    public partial class NewEventScreen : ContentPage
    {
        private Event __event;
        private Label lbl_id;
        private Entry ent_id;
        private Label lbl_description;
        private Entry ent_description;
        private Label lbl_type;
        private Entry ent_type;
        private Button btn_save;
        private Button btn_cancel;
        Label lbl_main;

        public NewEventScreen()
        {
            InitializeComponent();
            InitComponents();
            this.__event = null;
        }

        public NewEventScreen(Event __event)
        {
            InitializeComponent();
            InitComponents();
            this.__event = __event;
        }

        private void InitComponents()
        {
            //Inicializando componentes
            lbl_main = new Label();
            lbl_id = new Label();
            ent_id = new Entry();
            lbl_description = new Label();
            ent_description = new Entry();
            lbl_type = new Label();
            ent_type = new Entry();
            btn_save = new Button();
            btn_cancel = new Button();

            //Atribuindo propriedades
            if (__event != null)
                lbl_main.Text = Strings.Edit + " " + Strings.Event;
            else
                lbl_main.Text = Strings.New + " " + Strings.Event;
            lbl_main.HorizontalOptions = LayoutOptions.FillAndExpand;
            lbl_main.HorizontalTextAlignment = TextAlignment.Center;
            lbl_main.FontSize = 25;

            lbl_id.Text = Strings.ID;
            lbl_type.Text = Strings.Typeof;
            lbl_description.Text = Strings.Description;

            ent_id.HorizontalOptions = LayoutOptions.FillAndExpand;
            ent_description.HorizontalOptions = LayoutOptions.FillAndExpand;
            ent_type.HorizontalOptions = LayoutOptions.FillAndExpand;

            btn_save.Text = Strings.Save;
            btn_save.HorizontalOptions = LayoutOptions.FillAndExpand;
            btn_cancel.Text = Strings.Cancel;
            btn_cancel.HorizontalOptions = LayoutOptions.FillAndExpand;


            //Adicionando elementos ao layout
            StackLayout mainLayout = new StackLayout();
            mainLayout.Orientation = StackOrientation.Vertical;
            mainLayout.Children.Add(lbl_main);

            StackLayout stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(lbl_id);
            stack.Children.Add(ent_id);
            mainLayout.Children.Add(stack);

            stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(lbl_description);
            stack.Children.Add(ent_description);
            mainLayout.Children.Add(stack);
            
            stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(lbl_type);
            stack.Children.Add(ent_type);
            mainLayout.Children.Add(stack);           

            stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(btn_save);
            stack.Children.Add(btn_cancel);
            mainLayout.Children.Add(stack);

            this.Content = mainLayout;




        }
    }
}