using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Views.GameModes
{
    public class TDMGameModeView : GameModeView
    {
        public int PlayersPerTeam { get; set; }
        public override int TotalNumberOfPlayers { get { return PlayersPerTeam * 2; } }
    }
}
