using PhoneTag.SharedCodebase.Views.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PhoneTag.SharedCodebase.Models.GameModes
{
    public class VIPGameMode : GameMode
    {
        public int PlayersPerTeam { get; set; }

        public override int TotalNumberOfPlayers
        {
            get
            {
                return PlayersPerTeam * 2;
            }
        }

        public override async Task<dynamic> GenerateView()
        {
            VIPGameModeView view = new VIPGameModeView();

            view.PlayersPerTeam = PlayersPerTeam;
            
            return view;
        }

        public static VIPGameMode FromView(VIPGameModeView i_GameMode)
        {
            VIPGameMode gameMode = new VIPGameMode();

            gameMode.PlayersPerTeam = i_GameMode.PlayersPerTeam;

            return gameMode;
        }
    }
}