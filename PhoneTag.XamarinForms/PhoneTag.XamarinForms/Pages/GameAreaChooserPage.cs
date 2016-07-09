using PhoneTag.XamarinForms.Controls.MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// This page displays an interactive map that allows you to choose the game area while setting
    /// up a game.
    /// The chosen area is accessible via the static properties after returning from this page.
    /// </summary>
    public partial class GameAreaChooserPage : ContentPage
    {
        /// <summary>
        /// Holds the game location as chosen by the interactive map.
        /// </summary>
        public Position ChosenPosition { get; private set; }
        /// <summary>
        /// Holds the game radius as chosen by the interactive map.
        /// </summary>
        public double ChosenRadius { get; private set; }

        private const double k_DefaultGameRadius = 0.5;
        private const double k_DefaultGameZoom = 1;
        private const bool k_IsSetUpView = true;

        private GameMapSetup m_GameMap;
        
        public GameAreaChooserPage()
        {
            setupChooserMap();

            initializeComponent();
        }

        //Initializes the map to the last chosen location or to your current location.
        private void setupChooserMap()
        {
            Position startLocation = new Position(32.0486850, 34.7600850);
            m_GameMap = new GameMapSetup(startLocation, k_DefaultGameRadius, k_DefaultGameZoom);
           
            //Store the values in the static properties for access once we're done.
            ChosenPosition = startLocation;
            ChosenRadius = k_DefaultGameRadius;
        }

        //When the area is chosen return to the last page.
        private async void DoneButton_Clicked()
        {
            await Navigation.PopAsync();
        }
    }
}
