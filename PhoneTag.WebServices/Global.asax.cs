using com.shephertz.app42.paas.sdk.csharp;
using PhoneTag.SharedCodebase.Events.OpLogEvents;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.WebServices.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using PhoneTag.WebServices.Controllers.ExpirationControllers;
using PhoneTag.SharedCodebase.Controllers.ExpirationControllers;

namespace PhoneTag.WebServices
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            App42API.Initialize(Keys.App42APIKey, Keys.App42SecretKey);
            Mongo.Init();

            RoomExpirationController.InitRoomExpirationController();
            DisputeExpirationController.InitDisputeExpirationController();
            UserExpirationController.InitUserExpirationController();
            OpLogEventDispatcher.Init();
        }
    }
}
