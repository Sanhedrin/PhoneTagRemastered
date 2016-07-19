using MongoDB.Driver;
using PhoneTag.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PhoneTag.SharedCodebase.Controllers
{
    /// <summary>
    /// Controller to access service operations.
    /// </summary>
    public class ServiceController : ApiController
    {
        /// <summary>
        /// Checks if the service is currently available.
        /// </summary>
        // GET api/service
        public bool Get()
        {
            return Mongo.IsReady;
        }
    }
}