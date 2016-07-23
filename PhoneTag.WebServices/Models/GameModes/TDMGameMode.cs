using PhoneTag.SharedCodebase.Views.GameModes;
using PhoneTag.WebServices.Utilities;
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
            view.Teams = Teams;

            return view;
        }

        public static TDMGameMode FromView(TDMGameModeView i_GameMode)
        {
            TDMGameMode gameMode = new TDMGameMode();

            gameMode.PlayersPerTeam = i_GameMode.PlayersPerTeam;

            return gameMode;
        }

        /// <summary>
        /// Arranges the players in this game into 2 teams.
        /// </summary>
        public override void ArrangeTeams(List<string> i_LivingUsers)
        {
            List<String> team1 = new List<string>(), team2;

            List<String> usersToSplit = new List<String>(i_LivingUsers);

            for (int i = 0; i < i_LivingUsers.Count / 2; ++i)
            {
                int chosenUser = Randomizer.Range(usersToSplit.Count);

                team1.Add(usersToSplit[chosenUser]);

                usersToSplit.RemoveAt(chosenUser);
            }

            team2 = new List<String>(usersToSplit);

            Teams.Add(team1);
            Teams.Add(team2);
        }

        public override void GameStateUpdate()
        {
            
        }
    }
}