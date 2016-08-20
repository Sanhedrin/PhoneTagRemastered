    using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles;
using PhoneTag.XamarinForms.Pages;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.SocialMenu
{
    public class ShotSuggestionListDisplay : PlayerListDisplay
    {
        private bool m_ShowingAllTargets = false;

        public override PlayerListDisplay Refresh()
        {
            refresh();

            return this;
        }

        private async Task refresh()
        {
            IEnumerable<UserView> players = await getShotSuggestions();

            updatePlayerList(players, PlayerDetailsTileType.ShotSuggestions);
        }

        private async Task refreshFullDetails()
        {
            IEnumerable<UserView> players = await getFullShotSuggestions();

            updatePlayerList(players, PlayerDetailsTileType.ShotSuggestions);
        }

        //Gets all opposing living players.
        private async Task<IEnumerable<UserView>> getFullShotSuggestions()
        {
            List<UserView> shotSuggestions = new List<UserView>();

            GameRoomView roomView = await GameRoomView.GetRoom(UserView.Current.PlayingIn);

            if (roomView != null)
            {
                //IEnumerable<UserView> shotSuggestionEnum = roomView.LivingUsers.Where((user) => 
                //    { return !UserView.Current.FBID.Equals(user.FBID); });

                //if(shotSuggestionEnum != null && shotSuggestionEnum.Count() > 0)
                //{
                //    shotSuggestions = shotSuggestionEnum.ToList();
                //}
                shotSuggestions = roomView.LivingUsers;
            }
            else
            {
                Application.Current.MainPage = new ErrorPage("Could not fetch room info");
            }

            return shotSuggestions;
        }

        //Returns a collection of friend ids for the current player.
        private async Task<IEnumerable<UserView>> getShotSuggestions()
        {
            List<UserView> shotSuggestions = new List<UserView>();

            GameRoomView roomView = await GameRoomView.GetRoom(UserView.Current.PlayingIn);

            if (roomView != null)
            {
                Position position = await CrossGeolocator.Current.GetPositionAsync(1, includeHeading: true);
                shotSuggestions = await roomView.GetEnemiesInMySights(new GeoPoint(position.Latitude, position.Longitude), position.Heading);
            }
            else
            {
                Application.Current.MainPage = new ErrorPage("Could not fetch room info");
            }

            return shotSuggestions;
        }

        protected override async Task initializeComponent(PlayerDetailsTileType i_DetailType)
        {
            StackLayout friendList = await generatePlayerListPresenter(i_DetailType);

            if (!m_ShowingAllTargets)
            {
                //friendList.Children.Add(new Button()
                //{
                //    BackgroundColor = Color.Silver,
                //    Text = "Can't find my target",
                //    TextColor = Color.Black,
                //    Command = new Command(() => { refreshFullDetails(); })
                //});

                m_ShowingAllTargets = true;
            }

            Content = new StackLayout
            {
                Children = {
                    friendList
                }
            };
        }
    }
}
