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

            ScrollView scrollView = new ScrollView
            {
                Content = m_GameRoomTileDisplay
            };
            AbsoluteLayout.SetLayoutBounds(scrollView, new Rectangle(0, 0, 1, 0.9));
            AbsoluteLayout.SetLayoutFlags(scrollView, AbsoluteLayoutFlags.All);

            StackLayout bottomButtonBatch = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = new LayoutOptions { Alignment = LayoutAlignment.Fill },
                Children =
                {
                    new Label()
                    {
                        Text = "Search Radius"
                    },
                    pickerSearchRadius
                }
            };
            AbsoluteLayout.SetLayoutBounds(bottomButtonBatch, new Rectangle(0, 0.9, 1, 0.1));
            AbsoluteLayout.SetLayoutFlags(bottomButtonBatch, AbsoluteLayoutFlags.All);

            pickerSearchRadius.SelectedItem = oldSearchRadius.ToString();

            Button refreshButton = new Button
            {
                Text = "Refresh",
                Command = new Command(() => { IsEnabled = false; populateRoomList(); })
            };
            AbsoluteLayout.SetLayoutBounds(refreshButton, new Rectangle(0, 1, 1, 0.1));
            AbsoluteLayout.SetLayoutFlags(refreshButton, AbsoluteLayoutFlags.All);

            Title = "Find a game near you";
            Padding = new Thickness(0, 20, 0, 0);
            
            Content = new AbsoluteLayout()
            {
                VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Fill },
                Children = {
                    scrollView,
                    bottomButtonBatch,
                    refreshButton
                }
            };
        }

        private void initRadiusPicker()
        {
            pickerSearchRadius = new BindablePicker()
            {
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
