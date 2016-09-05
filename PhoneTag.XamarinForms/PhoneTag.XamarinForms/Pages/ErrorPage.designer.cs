using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class ErrorPage : TrailableContentPage
    {
        private void initializeComponent(string i_ErrorMessage)
        {
            BackgroundColor = Color.Black;
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
                        Text = i_ErrorMessage,
                        TextColor = Color.White
                    },
                    new Button{
                        Text = "Back to main menu",
                        TextColor = Color.White,
                        Command = new Command(() => { RestartAppButton_Clicked(); })
                    }
                }
            };
        }
    }
}
