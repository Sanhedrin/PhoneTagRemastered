using PhoneTag.SharedCodebase.Events.GameEvents;
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

            for (int i = 0; i < Math.Ceiling((float)i_LivingUsers.Count / 2); ++i)
            {
                int chosenUser = Randomizer.Range(usersToSplit.Count);

                team1.Add(usersToSplit[chosenUser]);

                usersToSplit.RemoveAt(chosenUser);
            }

            team2 = new List<String>(usersToSplit);

            Teams.Add(team1);
            Teams.Add(team2);
        }

        public override void GameStateUpdate(List<String> i_LivingUsers)
        {
            //If all members on this team are dead, the game ends.
            if(Teams[0].Intersect(i_LivingUsers).Count() == 0)
            {
                onGameEnded(new GameEndedEventArgs(new GameEndedEvent(Teams[1])));
            }
            else if(Teams[1].Intersect(i_LivingUsers).Count() == 0)
            {
                onGameEnded(new GameEndedEventArgs(new GameEndedEvent(Teams[0])));
            }
        }

        public override void TimeUp()
        {
            List<String> winningPlayers = Teams[0].Count > Teams[1].Count ? Teams[0] :
                (Teams[0].Count < Teams[1].Count ? Teams[1] : new List<String>());

            onGameEnded(new GameEndedEventArgs(new GameEndedEvent(winningPlayers)));
        }
    }
}