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
    public partial class GameSearchPage : TrailableContentPage
    {
        private int k_MaxSearchDistance = 100 * 10;
        
        private void initializeNoResultComponent()
        {
            BackgroundColor = Color.Black;

            int oldSearchRadius = SearchRadius;

            NavigationPage.SetHasBackButton(this, true);

            ScrollView scrollView = generateRoomScrollView(
                new Label()
                {
                    Text = "No rooms were found in the given search range.",
                    TextColor = Color.White
                });

            StackLayout bottomButtonBatch = generateSearchRadiusPicker(oldSearchRadius);

            Button refreshButton = generateRefreshButton();

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

        private void initializeComponent()
        {
            BackgroundColor = Color.Black;

            int oldSearchRadius = SearchRadius;

            NavigationPage.SetHasBackButton(this, true);

            m_GameRoomTileDisplay.VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Fill };

            ScrollView scrollView = generateRoomScrollView(m_GameRoomTileDisplay);

            StackLayout bottomButtonBatch = generateSearchRadiusPicker(oldSearchRadius);

            Button refreshButton = generateRefreshButton();

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

        private Button generateRefreshButton()
        {
            Button refreshButton = new Button
            {
                Text = "Refresh",
                TextColor = Color.Black,
                BackgroundColor = Color.Silver,
                Command = new Command(() => { IsEnabled = false; populateRoomList(); })
            };
            AbsoluteLayout.SetLayoutBounds(refreshButton, new Rectangle(0, 1, 1, 0.1));
            AbsoluteLayout.SetLayoutFlags(refreshButton, AbsoluteLayoutFlags.All);

            return refreshButton;
        }

        private StackLayout generateSearchRadiusPicker(int i_OldSearchRadius)
        {
            StackLayout bottomButtonBatch = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = new LayoutOptions { Alignment = LayoutAlignment.Fill },
                Children =
                {
                    new Label()
                    {
                        Text = "Search Radius",
                        TextColor = Color.White
                    },
                    pickerSearchRadius
                }
            };
            AbsoluteLayout.SetLayoutBounds(bottomButtonBatch, new Rectangle(0, 0.9, 1, 0.1));
            AbsoluteLayout.SetLayoutFlags(bottomButtonBatch, AbsoluteLayoutFlags.All);

            if (pickerSearchRadius.Items.Contains(i_OldSearchRadius.ToString()))
            {
                pickerSearchRadius.SelectedItem = i_OldSearchRadius.ToString();
            }

            return bottomButtonBatch;
        }

        private ScrollView generateRoomScrollView(View i_Content)
        {
            ScrollView scrollView = new ScrollView
            {
                Content = i_Content
            };
            AbsoluteLayout.SetLayoutBounds(scrollView, new Rectangle(0, 0, 1, 0.9));
            AbsoluteLayout.SetLayoutFlags(scrollView, AbsoluteLayoutFlags.All);

            return scrollView;
        }

        private void initRadiusPicker()
        {
            pickerSearchRadius = new BindablePicker()
            {
                Title = "Search Radius in Meters",
                BindingContext = this
            };
            pickerSearchRadius.Items.Clear();
            pickerSearchRadius.ItemsSource = (IList)pickerSearchRadius.Items;
            for (int i = 100; i <= k_MaxSearchDistance; i += 100)
            {
                pickerSearchRadius.Items.Add(i.ToString());
            }
            pickerSearchRadius.SetBinding(BindablePicker.SelectedItemProperty, "SearchRadius");
            SearchRadius = 300;
        }
    }
}
