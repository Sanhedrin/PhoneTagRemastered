using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.WebServices.Views;
using PhoneTag.WebServices.Views.GameModes;
using System.Reflection;

namespace PhoneTag.WebServices.Utils
{
    public static class GameModeFactory
    {
        private static readonly Dictionary<String, Type> sr_ModeObjects = new Dictionary<string, Type>()
        {
            { "Team Deathmatch", typeof(TDMGameModeView) },
            { "VIP", typeof(VIPGameModeView) }
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
    }
}
