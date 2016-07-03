using PhoneTag.SharedCodebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PhoneTag.WebServices.Controllers
{
    public class GameController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        //[Route("api/game/position")]
        //[HttpPost]
        //public void PositionUpdate([FromBody]Point r)
        //{
        //    Redis.Database.GeoAdd("Test", new GeoLocation { Name = "Player", Longitude = r.X, Latitude = r.Y });
        //}

        [Route("api/game/shoot")]
        [HttpPost]
        public string Shoot([FromBody]Point r)
        {
            return r.Y.ToString();
        }
    }
}
