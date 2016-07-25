using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Views;
using Xamarin.Forms.Maps;

namespace PhoneTag.XamarinForms.Controls.MapControl
{
    /// <summary>
    /// A custom control representing an interactive map that can be put as a view on a page.
    /// The map is initialized to a given play area and is locked to it, allowing players to view the game
    /// area, but not stray away from it to keep ease of use.
    /// </summary>
    public class GameMapInteractive : GameMapDisplay
    {
        public event EventHandler<PlayerLocationMarkerUpdateEventArgs> PlayerLocationMarkersUpdated;

        //We override the GameRadius and StartLocation properties of the game map since the setup map
        //allows setting to these values, whereas the play map doesn't.
        //This uses the protected members being hidden by the property to keep consistency of data between
        //the different levels of inheritance.
        public new double GameRadius { get { return m_GameRadius; } private set { m_GameRadius = value; } }
        public new Position StartLocation { get { return m_StartLocation; } private set { m_StartLocation = value; } }

        public GameMapInteractive(Position i_GameLocation, double i_GameRadius, double i_ZoomRadius) : base(i_GameLocation, i_GameRadius, i_ZoomRadius)
        {
        }

        /// <summary>
        /// Takes a given game room and player locations associated with the players in the room and passes
        /// marker information for each relevant player to the underlying map for display.
        /// </summary>
        public void UpdateUAV(Dictionary<string, GeoPoint> i_PlayersLocations, GameRoomView i_GameRoomView)
        {
            if (PlayerLocationMarkersUpdated != null)
            {
                List<Tuple<PlayerAllegiance, GeoPoint>> playerMarkers = new List<Tuple<PlayerAllegiance, GeoPoint>>();

                foreach (String userId in i_PlayersLocations.Keys)
                {
                    PlayerAllegiance allegiance = i_GameRoomView.GameDetails.Mode.GetAllegianceFor(userId);

                    playerMarkers.Add(new Tuple<PlayerAllegiance, GeoPoint>(allegiance, i_PlayersLocations[userId]));
                }

                PlayerLocationMarkersUpdated(this, new PlayerLocationMarkerUpdateEventArgs(playerMarkers));
            }
        }
    }
}
