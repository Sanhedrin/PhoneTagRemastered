using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class ErrorPage : ContentPage
    {
        private void initializeComponent(string i_ErrorMessage)
        {
            Title = "Error!";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Center
                },
                Children = {
                    new Label
                    {
                        Text = i_ErrorMessage
                    },
                    new Button{
                        Text = "Back to main menu",
                        Command = new Command(() => { RestartAppButton_Clicked(); })
                    }
                }
            };
        }
    }
}
