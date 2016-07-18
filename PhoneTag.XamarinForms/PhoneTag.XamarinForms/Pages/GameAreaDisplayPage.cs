using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Utils;
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
    /// Displays the map that was chosen for the current game without enabling any game features on it
    /// </summary>
    public partial class GameAreaDisplayPage : TrailableContentPage
    {        
        private const double k_DefaultZoomRatio = 2;

        private GameMapDisplay m_GameMap;

        public GameAreaDisplayPage(Position i_StartLocation, double i_Radius) : base()
        {
            setupChooserMap(i_StartLocation, i_Radius);

            initializeComponent();
        }

        //Initializes the map to the last chosen location or to your current location.
        private void setupChooserMap(Position i_StartLocation, double i_Radius)
        {
            m_GameMap = new GameMapDisplay(i_StartLocation, i_Radius, i_Radius * k_DefaultZoomRatio);
        }

        //When the area is chosen return to the last page.
        private async Task DoneButton_Clicked()
        {
            await Navigation.PopAsync();
        }

        public override void ParseEvent(Event i_EventDetails)
        {
            throw new NotImplementedException();
        }
    }
}
