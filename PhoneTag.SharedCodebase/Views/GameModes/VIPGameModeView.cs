using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.Utils;

namespace PhoneTag.SharedCodebase.Views.GameModes
{
    public class VIPGameModeView : GameModeView
    {
        public int PlayersPerTeam { get; set; }
        public List<string> VipForTeam { get; set; }

        public override int TotalNumberOfPlayers { get { return PlayersPerTeam * 2; } }

        public override PlayerAllegiance GetAllegianceFor(string i_UserId)
        {
            PlayerAllegiance allegiance;

            if (i_UserId.Equals(UserView.Current.FBID))
            {
                allegiance = VipForTeam.Contains(UserView.Current.FBID) ? PlayerAllegiance.SpecialSelf : PlayerAllegiance.Self;
            }
            else
            {
                List<String> playerTeam = GetPlayerTeamFor(i_UserId);

                if (playerTeam.Contains(UserView.Current?.FBID))
                {
                    allegiance = VipForTeam.Contains(i_UserId) ? PlayerAllegiance.Special : PlayerAllegiance.Ally;
                }
                else
                {
                    allegiance = PlayerAllegiance.Enemy;
                }
            }

            return allegiance;
        }

        public override List<string> GetPlayerTeamFor(string i_UserId)
        {
            List<String> team = new List<string>();

            for (int i = 0; i < Teams.Count; ++i)
            {
                if (Teams[i].Contains(i_UserId))
                {
                    team = Teams[i];
                    break;
                }
            }

            return team;
        }
    }
}
