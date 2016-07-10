﻿using PhoneTag.SharedCodebase.Views;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.GameDetailsTile
{
    public class GameDetailsTile : StackLayout
    {
        private String m_GameRoomId;

        public GameDetailsTile(String i_GameRoomId)
        {
            m_GameRoomId = i_GameRoomId;
            setupTile();
        }

        private async void setupTile()
        {
            GameRoomView room = await GameRoomView.GetRoom(m_GameRoomId);

            Orientation = StackOrientation.Horizontal;
            HorizontalOptions = LayoutOptions.FillAndExpand;

            //The game's name goes on the button, clicking tries to join the room.
            Children.Add(new Button() {
                Text = room.GameDetails.Name,
                BackgroundColor = Color.Green,
                WidthRequest = CrossScreen.Current.Size.Width / 4,
                Command = new Command(() => { joinRoom(); })
            });

            Children.Add(new Label() {
                BackgroundColor = Color.Gray,
                TextColor = Color.Black,
                Text = constructDetails(room),
                WidthRequest = CrossScreen.Current.Size.Width * 3 / 4
            });
        }

        //Builds a string that displays relevant details about a room.
        private string constructDetails(GameRoomView i_Room)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(i_Room.GameDetails.Mode.Name);
            builder.AppendLine(String.Format("{0} minutes", i_Room.GameDetails.GameDurationInMins));
            builder.AppendLine(String.Format("{0} players / {1} friends", 
                i_Room.LivingUsers.Count.ToString(), i_Room.FriendsInRoomFor(UserView.Current).Count.ToString()));

            return builder.ToString();
        }

        //Checks if we can join this room, and if so, loads up the lobby page.
        private void joinRoom()
        {
            throw new NotImplementedException();
        }
    }
}
