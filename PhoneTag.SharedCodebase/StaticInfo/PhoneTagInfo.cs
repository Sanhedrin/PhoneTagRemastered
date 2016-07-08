using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.StaticInfo
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

            return String.Equals(ClientVersion, serverVersion);
        }

        /// <summary>
        /// Gets the names of all game modes supported by the server.
        /// </summary>
        public static async Task<List<String>> GetGameModeList()
        {
            using (HttpClient client = new HttpClient())
            {
                List<String> gameModes = await client.GetMethodAsync<List<String>>("info/game_modes");

                return gameModes;
            }
        }
    }
}
