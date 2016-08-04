using PhoneTag.XamarinForms.Extensions;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class CreateGamePage : TrailableContentPage
    {
        private void initializeComponent()
        {
            NavigationPage.SetHasBackButton(this, true);

            textBoxGameName.WidthRequest = CrossScreen.Current.Size.Width;
            textBoxGameName.Placeholder = "Game Name";
            textBoxGameName.SetBinding(Entry.TextProperty, "Name");
            textBoxGameName.BindingContext = m_GameDetails;
            textBoxGameName.Keyboard = Keyboard.Text;

            pickerGameMode.WidthRequest = CrossScreen.Current.Size.Width;
            pickerGameMode.Title = "Choose a Game Mode";
            pickerGameMode.IsEnabled = false;

            buttonCreateGame.Text = "Create";
            buttonCreateGame.TextColor = Color.Black;
            buttonCreateGame.IsEnabled = false;
            buttonCreateGame.BackgroundColor = Color.Red;
            
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
                        TextColor = Color.Black,
                        BackgroundColor = Color.Yellow,
                        Command = new Command(() => { SetGameAreaButton_Clicked(); })
                    },
                    stackLayoutGameDetails,
                    buttonCreateGame
                }
            };
        }
    }
}
