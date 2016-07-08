using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class MainMenuPage: ContentPage
    {
        private void initializeComponent()
        {
            Title = "Main Menu";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                Children = {
                    //TODO: Insert title image asset.
                    new Button()
                    {
                        Text = "Find Game In Your Area!",
                        BackgroundColor = Color.Yellow,
                        Command = new Command(() => { FindGameButton_Clicked(); })
                    },
                    new Button()
                    {
                        Text = "Create Game!",
                        BackgroundColor = Color.Green,
                        Command = new Command(() => { CreateGameButton_Clicked(); })
                    }
                    //TODO: Add friend list component here.
                }
            };
        }
    }
}
