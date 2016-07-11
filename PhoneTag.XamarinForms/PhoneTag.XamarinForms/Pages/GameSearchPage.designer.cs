using FreshEssentials;
using PhoneTag.XamarinForms.Controls.GameDetailsTile;
using Plugin.DeviceInfo;
using Plugin.XamJam.Screen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class GameSearchPage : ContentPage
    {
        private int k_MaxSearchDistance = 10;

        private void initializeComponent()
        {
            int oldSearchRadius = SearchRadius;

            NavigationPage.SetHasBackButton(this, true);

            m_GameRoomTileDisplay.VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Fill };
            
            Title = "Find a game near you";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout()
            {
                VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Fill },
                Children = {
                    new ScrollView
                    {
                        HeightRequest = CrossScreen.Current.Size.Height * 3 / 4,
                        WidthRequest = CrossScreen.Current.Size.Width,
                        Content = m_GameRoomTileDisplay
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = new LayoutOptions { Alignment = LayoutAlignment.Fill },
                        Children =
                        {
                            new Label()
                            {
                                Text = "Search Radius",
                                WidthRequest = CrossScreen.Current.Size.Width / 4,
                                HeightRequest = CrossScreen.Current.Size.Height / 16
                            },
                            pickerSearchRadius
                        }
                    },
                    new Button
                    {
                        Text = "Refresh",
                        WidthRequest = CrossScreen.Current.Size.Width,
                        HeightRequest = CrossScreen.Current.Size.Height / 16,
                        Command = new Command(() => { populateRoomList(); })
                    }
                }
            };

            pickerSearchRadius.SelectedItem = oldSearchRadius.ToString();
        }

        private void initRadiusPicker()
        {
            pickerSearchRadius = new BindablePicker()
            {
                WidthRequest = CrossScreen.Current.Size.Width * 3 / 4,
                HeightRequest = CrossScreen.Current.Size.Height / 16,
                Title = "Search Radius",
                BindingContext = this
            };
            pickerSearchRadius.Items.Clear();
            pickerSearchRadius.ItemsSource = (IList)pickerSearchRadius.Items;
            for (int i = 1; i <= k_MaxSearchDistance; ++i)
            {
                pickerSearchRadius.Items.Add(i.ToString());
            }
            pickerSearchRadius.SetBinding(BindablePicker.SelectedItemProperty, "SearchRadius");
            SearchRadius = 3;
        }
    }
}
