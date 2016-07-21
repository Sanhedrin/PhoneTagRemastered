using PhoneTag.SharedCodebase.Views.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PhoneTag.WebServices.Models.GameModes
{
    public class TDMGameMode : GameMode
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
            TDMGameModeView view = new TDMGameModeView();

            view.PlayersPerTeam = PlayersPerTeam;

            return view;
        }

        public static TDMGameMode FromView(TDMGameModeView i_GameMode)
        {
            TDMGameMode gameMode = new TDMGameMode();

            gameMode.PlayersPerTeam = i_GameMode.PlayersPerTeam;

            return gameMode;
        }
    }
}