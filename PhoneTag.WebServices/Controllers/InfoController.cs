using PhoneTag.SharedCodebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using PhoneTag.WebServices.Models;
using PhoneTag.SharedCodebase.Views;
using System.Linq.Expressions;
using PhoneTag.WebServices;
using PhoneTag.WebServices.Utilities;

namespace PhoneTag.SharedCodebase.Controllers
{
    /// <summary>
    /// Obtains statics information about the server that isn't related to any specific instances.
    /// </summary>
    public class InfoController : ApiController
    {
        /// <summary>
        /// Gets a list of the supported game mode names.
        /// </summary>
        [Route("api/info/game_modes")]
        [HttpGet]
        public async Task<List<String>> GetGameModeList()
        {
            List<String> gameModeNames = new List<String>();

            try
            {
                IFindFluent<GameMode, String> gameModes = Mongo.Database.GetCollection<GameMode>("GameModes")
                    .Find(Builders<GameMode>.Filter.Empty)
                    .Project(gameMode => gameMode.Name);

                if (gameModes.Count() > 0)
                {
                    gameModeNames = await gameModes.ToListAsync();
                }
            }
            catch (Exception e)
            {
                gameModeNames = null;
                ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
            }

            return gameModeNames;
        }

        /// <summary>
        /// Gets the current server version, should match the client version to allow connection.
        /// </summary>
        [Route("api/info/version")]
        [HttpGet]
        public async Task<String> GetVersion()
        {
            String version = null;

            try
            {
                IFindFluent<ServerInfo, String> serverInfo = Mongo.Database.GetCollection<ServerInfo>("Info")
                    .Find(Builders<ServerInfo>.Filter.Eq("Type", "ServerInfo"))
                    .Project(info => info.Version);
                version = await serverInfo.FirstAsync();
            }
            catch (Exception e)
            {
                version = null;
                ErrorLogger.Log(String.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
            }

            return version;
        }
    }
}