using PhoneTag.WebServices.Models;
using PhoneTag.SharedCodebase.Views.GameModes;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using PhoneTag.WebServices.Utilities;
using System.Linq;
using PhoneTag.SharedCodebase.Events.GameEvents;

namespace PhoneTag.WebServices.Models.GameModes
{
    public class VIPGameMode : GameMode
    {
        public int PlayersPerTeam { get; set; }
        public List<string> VipForTeam { get; private set; }

        public override int TotalNumberOfPlayers
        {
            get
            {
                return PlayersPerTeam * 2;
            }
        }

        public VIPGameMode() : base()
        {
            VipForTeam = new List<String>();
        }

        public override async Task<dynamic> GenerateView()
        {
            VIPGameModeView view = new VIPGameModeView();

            view.PlayersPerTeam = PlayersPerTeam;
            view.Teams = Teams;
            view.VipForTeam = VipForTeam;

            return view;
        }

        public static VIPGameMode FromView(VIPGameModeView i_GameMode)
        {
            VIPGameMode gameMode = new VIPGameMode();

            gameMode.PlayersPerTeam = i_GameMode.PlayersPerTeam;

            return gameMode;
        }

        /// <summary>
        /// Arranges the players in this game into 2 teams and sets a VIP for each team.
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

            //Once the teams are set up, we choose a VIP for each side.
            foreach (List<String> team in Teams)
            {
                if (team.Count > 0)
                {
                    VipForTeam.Add(team[Randomizer.Range(team.Count)]);
                }
                else
                {
                    VipForTeam.Add(null);
                }
            }
        }

        public override void GameStateUpdate(List<String> i_LivingUsers)
        {
            //If all members on this team are dead, or the VIP was killed, the game ends.
            if (i_LivingUsers.Intersect(Teams[0]).Count() == 0 || !i_LivingUsers.Contains(VipForTeam[0]))
            {
                onGameEnded(new GameEndedEventArgs(new GameEndedEvent(Teams[1])));
            }
            else if (i_LivingUsers.Intersect(Teams[1]).Count() == 0 || !i_LivingUsers.Contains(VipForTeam[1]))
            {
                onGameEnded(new GameEndedEventArgs(new GameEndedEvent(Teams[0])));
            }
        }
    }
}