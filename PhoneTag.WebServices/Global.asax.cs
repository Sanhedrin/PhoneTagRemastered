using com.shephertz.app42.paas.sdk.csharp;
using PhoneTag.SharedCodebase.Events.OpLogEvents;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using PhoneTag.SharedCodebase.Controllers.ExpirationControllers;

namespace PhoneTag.SharedCodebase
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            App42API.Initialize(Keys.App42APIKey, Keys.App42SecretKey);
            Mongo.Init();

            RoomExpirationController.InitRoomExpirationController();
            UserExpirationController.InitUserExpirationController();
            OpLogEventDispatcher.Init();
        }
    }
}
