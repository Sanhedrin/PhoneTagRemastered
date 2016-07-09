using PhoneTag.SharedCodebase.Views.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhoneTag.WebServices.Models.GameModes
{
    public class VIPGameMode : GameMode
    {
        public override dynamic GenerateView()
        {
            VIPGameModeView view = new VIPGameModeView();
            
            return view;
        }

        public static VIPGameMode FromView(VIPGameModeView i_GameMode)
        {
            VIPGameMode gameMode = new VIPGameMode();

            return gameMode;
        }
    }
}