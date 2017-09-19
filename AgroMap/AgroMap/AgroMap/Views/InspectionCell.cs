using AgroMap.Entity;
using AgroMap.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgroMap.Views
{
    class InspectionCell: ViewCell
    {
        

        public InspectionCell(InspectionScreen page)
        {

            //Inicializando componentes
            StackLayout mainLayout = new StackLayout();
            StackLayout firstLine = new StackLayout();
            StackLayout secondLine = new StackLayout();
            StackLayout thirdLine = new StackLayout();
            StackLayout fourthLine = new StackLayout();
            Label lblID = new Label();
            Label lblIDContent = new Label();
            Label lblName = new Label();
            Label lblNameContent = new Label();
            Label lblStart = new Label();
            Label lblStartContent = new Label();
            Label lblEnd = new Label();
            Label lblEndContent = new Label();


            //Atribuindo valores e bindings
            lblID.Text = Strings.ID;
            lblIDContent.SetBinding(Label.TextProperty, "Id");
            lblName.Text = Strings.Name;
            lblNameContent.SetBinding(Label.TextProperty, "Name");
            lblStart.Text = Strings.StartAt;
            lblStartContent.SetBinding(Label.TextProperty, "Start_At");
            lblEnd.Text = Strings.EndAt;
            lblEndContent.SetBinding(Label.TextProperty, "End_At");

            //Propriedades de Layout
            mainLayout.BackgroundColor = Color.FromHex ("#eee");
            mainLayout.Orientation = StackOrientation.Vertical;
            firstLine.Orientation = StackOrientation.Horizontal;
            secondLine.Orientation = StackOrientation.Horizontal;
            thirdLine.Orientation = StackOrientation.Horizontal;
            fourthLine.Orientation = StackOrientation.Horizontal;
            

            //Adicionando itens aos layouts
            firstLine.Children.Add(lblID);
            firstLine.Children.Add(lblIDContent);

            secondLine.Children.Add(lblName);
            secondLine.Children.Add(lblNameContent);

            thirdLine.Children.Add(lblStart);
            thirdLine.Children.Add(lblStartContent);

            fourthLine.Children.Add(lblEnd);
            fourthLine.Children.Add(lblEndContent);

            mainLayout.Children.Add(firstLine);
            mainLayout.Children.Add(secondLine);
            mainLayout.Children.Add(thirdLine);
            mainLayout.Children.Add(fourthLine);


            var details = new MenuItem { Text = Strings.Details };
            details.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            details.Clicked += async (sender, e) =>
            {
                page.ListView_Inspection_Details(((Inspection)BindingContext));
            };

            var members = new MenuItem { Text = Strings.ShowOnMap };
            members.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            members.Clicked += async (sender, e) => {
                page.ListView_Inspection_Members(((Inspection)BindingContext));
            };

            var edit = new MenuItem { Text = Strings.Edit };
            edit.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            edit.Clicked += async (sender, e) => {
                page.ListView_Inspection_Edit(((Inspection)BindingContext));
                
            };

            var delete = new MenuItem { Text = Strings.Delete };
            delete.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            delete.Clicked += async (sender, e) => {
                page.ListView_Inspection_Delete(((Inspection)BindingContext));
            };
            

            ContextActions.Add(details);
            ContextActions.Add(members);
            ContextActions.Add(edit);
            ContextActions.Add(delete);

            View = mainLayout;


        }
    }
}
