using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

[assembly: OwinStartup(typeof(PhoneTag.SharedCodebase.OwinConfig))]

namespace PhoneTag.SharedCodebase
{
    public class OwinConfig
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            new MobileAppConfiguration()
                .AddPushNotifications()
                .ApplyTo(config);

            app.UseWebApi(config);
        }
    }
}