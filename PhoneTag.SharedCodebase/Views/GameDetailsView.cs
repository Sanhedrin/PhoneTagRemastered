using PhoneTag.SharedCodebase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Views
{
    public class GameDetailsView
    {
        public String Name { get; set; }
        public int GpsRefreshRate { get; set; }
        public int GameDurationInMins { get; set; }
        public GeoPoint StartLocation { get; set; }
        public double GameRadius { get; set; }

        public GameModeView Mode { get; set; }

        public GameDetailsView()
        {

        }

        public GameDetailsView(string i_GameModeName)
        {
            Mode = GameModeFactory.GetModeView(i_GameModeName);
        }
    }
}
