using PhoneTag.SharedCodebase.Views.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhoneTag.WebServices.Models.GameModes
{
    public class TDMGameMode : GameMode
    {
        public int PlayersPerTeam { get; set; }
        
        public override dynamic GenerateView()
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