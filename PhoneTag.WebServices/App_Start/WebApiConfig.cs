using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PhoneTag.WebServices
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //Sets the deserializer to use complete type info when neccassery so that we can use
            //abstract classes in our serializations.
            config.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}/{action}/{param}",
                defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional, param = RouteParameter.Optional }
            );
        }
    }
}
