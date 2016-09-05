using FreshEssentials;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.SharedCodebase.Views.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Extensions
{
    /// <summary>
    /// Contains extensions for generating presentable entry formats for the game details and modes.
    /// </summary>
    public static class GameDetailsPresenters
    {
        private const int k_HalfHourInSeconds = 60 * 30;
        private static int k_MaxPlayersPerTeam = 10;
        private static int k_MaxMinutesPerGame = 120;

        /// <summary>
        /// Generates a layout that allows editing the base values of this game's details.
        /// </summary>
        public static StackLayout GetViewPresenter(this GameDetailsView i_DetailsView)
        {
            StackLayout layout = new StackLayout()
            {
                BackgroundColor = Color.Gray,
                VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Fill }
            };



            ///Sets a picker for the duration of the game that binds to the field.
            layout.Children.Add(
                new Label()
                {
                    Text = "Game Duration in minutes:",
                    TextColor = Color.White
                });
            BindablePicker durationPicker = new BindablePicker()
            {
                Title = "Game Duration",
                BackgroundColor = Color.Black,
                TextColor = Color.White
            };
            durationPicker.ItemsSource = new List<int>();
            for (int i = 5; i <= k_MaxMinutesPerGame; i += 5) { durationPicker.ItemsSource.Add(i); durationPicker.Items.Add(i.ToString()); }
            durationPicker.SetBinding(BindablePicker.SelectedItemProperty, "GameDurationInMins");
            durationPicker.BindingContext = i_DetailsView;
            durationPicker.SelectedItem = 30;
            layout.Children.Add(durationPicker);



            //Adds the details required by the specific game modes.
            dynamic mode = i_DetailsView.Mode;
            foreach(View view in GetPresenterList(mode))
            {
                layout.Children.Add(view);
            }



            ///Sets a picker for the gps interval time that binds to the field.
            layout.Children.Add(
                new Label()
                {
                    Text = "Seconds per player location update on the map:",
                    TextColor = Color.White
                });
            BindablePicker uavPicker = new BindablePicker()
            {
                Title = "Seconds per player location update on the map",
                BackgroundColor = Color.Black,
                TextColor = Color.White
            };
            uavPicker.ItemsSource = new List<int>();
            for (int i = 10; i <= k_HalfHourInSeconds; i += 10) { uavPicker.ItemsSource.Add(i); uavPicker.Items.Add(i.ToString()); }
            uavPicker.SetBinding(BindablePicker.SelectedItemProperty, "GpsRefreshRate");
            uavPicker.BindingContext = i_DetailsView;
            uavPicker.SelectedItem = 60;
            layout.Children.Add(uavPicker);

            return layout;
        }

        /// <summary>
        /// Generates the list of items required to edit the base values for the game rules.
        /// </summary>
        public static List<View> GetPresenterList(this GameModeView i_ModeView)
        {
            List<View> items = new List<View>();
            
            return items;
        }

        /// <summary>
        /// Generates the list of items required to edit the base values for the game rules for VIP.
        /// </summary>
        public static List<View> GetPresenterList(this VIPGameModeView i_ModeView)
        {
            List<View> items = ((GameModeView)i_ModeView).GetPresenterList();

            items.Add(
                new Label()
                {
                    Text = "Players per team:",
                    TextColor = Color.White
                });

            //Sets a picker for the number of players per team in a TDM game.
            BindablePicker playersPerTeamPicker = new BindablePicker()
            {
                Title = "Number of players per team",
                BackgroundColor = Color.Black,
                TextColor = Color.White
            };
            playersPerTeamPicker.ItemsSource = new List<int>();
            for (int i = 1; i <= k_MaxPlayersPerTeam; i++) { playersPerTeamPicker.ItemsSource.Add(i); playersPerTeamPicker.Items.Add(i.ToString()); }
            playersPerTeamPicker.SetBinding(BindablePicker.SelectedItemProperty, "PlayersPerTeam");
            playersPerTeamPicker.BindingContext = i_ModeView;
            playersPerTeamPicker.SelectedItem = 3;
            items.Add(playersPerTeamPicker);

            return items;
        }

        /// <summary>
        /// Generates the list of items required to edit the base values for the game rules for TDM.
        /// </summary>
        public static List<View> GetPresenterList(this TDMGameModeView i_ModeView)
        {
            List<View> items = ((GameModeView)i_ModeView).GetPresenterList();
            
            items.Add(
                new Label()
                {
                    Text = "Players per team:",
                    TextColor = Color.White
                });

            //Sets a picker for the number of players per team in a TDM game.
            BindablePicker playersPerTeamPicker = new BindablePicker()
            {
                Title = "Number of players per team",
                BackgroundColor = Color.Black,
                TextColor = Color.White
            };
            playersPerTeamPicker.ItemsSource = new List<int>();
            for (int i = 1; i <= k_MaxPlayersPerTeam; i++) { playersPerTeamPicker.ItemsSource.Add(i); playersPerTeamPicker.Items.Add(i.ToString()); }
            playersPerTeamPicker.SetBinding(BindablePicker.SelectedItemProperty, "PlayersPerTeam");
            playersPerTeamPicker.BindingContext = i_ModeView;
            playersPerTeamPicker.SelectedItem = 3;
            items.Add(playersPerTeamPicker);

            return items;
        }
    }
}
