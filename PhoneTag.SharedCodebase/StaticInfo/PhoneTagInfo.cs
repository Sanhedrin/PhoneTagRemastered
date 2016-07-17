using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.WebServices.Views;

namespace PhoneTag.WebServices.StaticInfo
{
    /// <summary>
    /// Contains all static info about the game that isn't tied to any specific instances but 
    /// for the game in general.
    /// </summary>
    public static class PhoneTagInfo
    {
        public const string ClientVersion = "1.0.0.0";

        /// <summary>
        /// Gets the application's version as stored on the server
        /// </summary>
        public static async Task<String> GetServerVersion()
        {
            using (HttpClient client = new HttpClient())
            {
                String serverVersion = await client.GetMethodAsync<String>("info/version");

                return serverVersion;
            }
        }

        /// <summary>
        /// Makes sure that the client version is up to date with the server.
        /// </summary>
        /// <returns>True if the client is up to date, or false if an update is required.</returns>
        public static async Task<bool> ValidateVersion()
        {
            String serverVersion = await GetServerVersion();

            if (serverVersion == null)
            {
                throw new Exception(String.Format("Could not reach game server.{0}Please check your connection or try again later.", Environment.NewLine));
            }

            return String.Equals(ClientVersion, serverVersion);
        }

        /// <summary>
        /// Gets the names of all game modes supported by the server.
        /// </summary>
        //We hold the game mode list on the server, unlike the game mode objects, so that we have server
        //side control over which game modes are currently available to choose from without needing to
        //patch the client every time we want to make a change.
        public static async Task<List<String>> GetGameModeList()
        {
            using (HttpClient client = new HttpClient())
            {
                List<String> gameModes = await client.GetMethodAsync<List<String>>("info/game_modes");

                return gameModes;
            }
        }

        /// <summary>
        /// Gets a GameDetailsView for the wanted game mode that should be filled and sent to the server
        /// when creating a room.
        /// </summary>
        public static GameDetailsView GetGameDetailsForMode(string i_GameModeName)
        {
            return new GameDetailsView(i_GameModeName);
        }
    }
}
