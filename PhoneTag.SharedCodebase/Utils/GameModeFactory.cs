using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.SharedCodebase.Views.GameModes;
using System.Reflection;

namespace PhoneTag.SharedCodebase.Utils
{
    public static class GameModeFactory
    {
        private static readonly Dictionary<String, Type> sr_ModeObjects = new Dictionary<string, Type>()
        {
            { "Team Deathmatch", typeof(TDMGameModeView) },
            { "VIP", typeof(VIPGameModeView) }
        };

        private static readonly Dictionary<String, String> sr_ModeDescriptions = new Dictionary<string, string>()
        {
            { "Team Deathmatch", getTDMDescription() },
            { "VIP", getVIPDescription() }
        };

        //Makes sure that all the types entered in the above dictionary are types of game mode objects.
        static GameModeFactory()
        {
            foreach(Type type in sr_ModeObjects.Values)
            {
                if(!type.GetTypeInfo().IsSubclassOf(typeof(GameModeView)))
                {
                    System.Diagnostics.Debug.WriteLine("Game mode type list contains an invalid mode object");
                    throw new Exception("Game mode type list contains an invalid mode object");
                }
            }
        }

        /// <summary>
        /// Gets the name assigned to the given game mode type.
        /// </summary>
        /// <param name="type">Type of view of a mode.</param>
        /// <returns></returns>
        public static string GetNameOfModeViewType(Type i_GameModeViewType)
        {
            String foundName = null;

            foreach(KeyValuePair<String, Type> typeNamePair in sr_ModeObjects)
            {
                if(typeNamePair.Value.Equals(i_GameModeViewType))
                {
                    foundName = typeNamePair.Key;
                    break;
                }
            }

            return foundName;
        }

        /// <summary>
        /// Gets a new view object for the given game mode name.
        /// </summary>
        public static GameModeView GetModeView(string i_GameModeName)
        {
            GameModeView gameModeView = null;

            if (sr_ModeObjects.ContainsKey(i_GameModeName))
            {
                //This is guaranteed to work since our static constructor checks for inheritance of all
                //types in the object list.
                gameModeView = (GameModeView)Activator.CreateInstance(sr_ModeObjects[i_GameModeName]);
            }

            return gameModeView;
        }

        public static string GetDescriptionForMode(string i_ModeName)
        {
            return sr_ModeDescriptions.ContainsKey(i_ModeName) ? sr_ModeDescriptions[i_ModeName] : "Invalid game mode";
        }

        private static String getVIPDescription()
        {
            return String.Format("VIP is a spin on Team Deathmatch where 2 teams of equal size are faced against eachother.{0}In VIP, each team is assigned a VIP out of the participating players.{0}The winning team is the one that manages to kill their enemies' VIP first.", Environment.NewLine);
        }

        private static String getTDMDescription()
        {
            return String.Format("Team Deathmatch is a simple team based game where players are divided into 2 teams of equal sizes.{0}The last team left with living player wins.", Environment.NewLine);
        }
    }
}
