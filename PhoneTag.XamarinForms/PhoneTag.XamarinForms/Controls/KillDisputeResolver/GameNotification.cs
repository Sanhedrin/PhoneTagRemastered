using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.KillDisputeResolver
{
    public partial class GameNotification : StackLayout
    {
        public GameNotification(String i_NotificationMessage)
        {
            initializeComponent(i_NotificationMessage);
        }
    }
}
