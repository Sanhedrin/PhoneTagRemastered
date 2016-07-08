using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class CreateGamePage : ContentPage
    {
        private void initializeComponent()
        {
            NavigationPage.SetHasBackButton(this, true);

            textBoxGameName.WidthRequest = CrossScreen.Current.Size.Width;
            textBoxGameName.Placeholder = "Game Name";

            pickerGameMode.WidthRequest = CrossScreen.Current.Size.Width;
            pickerGameMode.Title = "Choose a Game Mode";

            Title = "Create Game";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                Children = {
                    pickerGameMode,
                    textBoxGameName,
                    new Button
                    {
                        Text = "Set Game Area",
                        BackgroundColor = Color.Yellow,
                        Command = new Command(() => { CreateGamePage_SetGameAreaButtonClicked(); })
                    },
                    //TODO: Add varying rules component
                    new Button
                    {
                        Text = "Create",
                        BackgroundColor = Color.Red,
                        Command = new Command(() => { CreateGamePage_CreateGameButtonClicked(); })
                    }
                }
            };
        }
    }
}
