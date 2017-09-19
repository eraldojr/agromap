using AgroMap.Entity;
using AgroMap.Resources;
using AgroMap.Services;
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
    public partial class NewInspectionScreen : ContentPage
    {
        private Inspection inspection = null;
        private Label lbl_main;
        private Label lbl_id;
        private Label lbl_id_content;
        private Label lbl_name;
        private Entry ent_name;
        private Label lbl_start;
        private DatePicker pck_start;
        private Label lbl_end;
        private DatePicker pck_end;
        //private Label lbl_members;
        //private Label lbl_members_content;
        
        private Button btn_save;
        private Button btn_cancel;

       
        public NewInspectionScreen()
        {
            InitializeComponent();
            InitComponents();
        }

        public NewInspectionScreen(Inspection inspection)
        {
            InitializeComponent();
            this.inspection = inspection;
            InitComponents();
        }

        private void InitComponents()
        {

            lbl_main = new Label();
            lbl_main.Text = Strings.NewInspection;
            lbl_main.FontSize = 25;
            lbl_main.HorizontalOptions = LayoutOptions.FillAndExpand;
            lbl_main.HorizontalTextAlignment = TextAlignment.Center;

            
            

            lbl_name = new Label();
            lbl_name.Text = Strings.Name;

            ent_name = new Entry();
            ent_name.HorizontalOptions = LayoutOptions.FillAndExpand;


            lbl_start = new Label();
            lbl_start.Text = Strings.StartAt;

            pck_start = new DatePicker();
            pck_start.HorizontalOptions = LayoutOptions.FillAndExpand;

            lbl_end = new Label();
            lbl_end.Text = Strings.EndAt;

            pck_end = new DatePicker();
            pck_end.HorizontalOptions = LayoutOptions.FillAndExpand;

            btn_save = new Button();
            btn_save.Text = Strings.Save;
            btn_save.Clicked += Btn_save_Clicked;

            btn_cancel = new Button();
            btn_cancel.Text = Strings.Cancel;
            btn_cancel.Clicked += Btn_cancel_Clicked;


            StackLayout mainLayout = new StackLayout();
            mainLayout.Orientation = StackOrientation.Vertical;
            mainLayout.Padding = 5;
            mainLayout.Children.Add(lbl_main);

            StackLayout stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;

            if (this.inspection != null)
            {
                lbl_id = new Label();
                lbl_id.Text = Strings.ID;
                lbl_id_content = new Label();

                stack.Children.Add(lbl_id);
                stack.Children.Add(lbl_id_content);

                LoadValues();

                mainLayout.Children.Add(stack);

            }

            stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(lbl_name);
            stack.Children.Add(ent_name);
            mainLayout.Children.Add(stack);

            stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(lbl_start);
            stack.Children.Add(pck_start);
            mainLayout.Children.Add(stack);

            stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(lbl_start);
            stack.Children.Add(pck_start);
            stack.Children.Add(lbl_end);
            stack.Children.Add(pck_end);
            mainLayout.Children.Add(stack);

            stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(btn_save);
            stack.Children.Add(btn_cancel);
            mainLayout.Children.Add(stack);

            mainLayout.Children.Add(btn_save);
            mainLayout.Children.Add(btn_cancel);

            this.Content = mainLayout;


        }

        private async void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void Btn_save_Clicked(object sender, EventArgs e)
        {

            if (inspection != null)
            {
                inspection.Name = ent_name.Text;
                inspection.Start_At = pck_start.Date;
                inspection.End_At = pck_end.Date;
            }
            else
            {
                this.inspection = new Inspection
                {
                    Id = 0,
                    Name = ent_name.Text,
                    Start_At = pck_start.Date,
                    End_At = pck_end.Date
                };
            }
            
            Boolean result = await InspectionService.Create(this.inspection);
            if (result)
            {
                //SUccess

            }
            else
            {
                //Fail
            }
        }

        private void LoadValues()
        {
            lbl_main.Text = Strings.EditInspection;
            lbl_id_content.Text = this.inspection.Id.ToString();
            ent_name.Text = this.inspection.Name;
            pck_start.Date = this.inspection.Start_At;
            pck_end.Date = this.inspection.End_At;

        }
    }
}