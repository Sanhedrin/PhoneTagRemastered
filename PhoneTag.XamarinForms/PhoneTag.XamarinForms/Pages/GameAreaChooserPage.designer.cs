using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class GameAreaChooserPage : ContentPage
    {
        private void initializeComponent()
        {
            NavigationPage.SetHasBackButton(this, true);

            Title = "Choose the game area";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                Children = {
                    m_GameMap,
                    new Button
                    {
                        Text = "Done",
                        Command = new Command(() => { GameAreaChooserPage_DoneButtonClicked(); })
                    }
                }
            };
        }
    }
}
