using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.KillDisputeResolver
{
    public partial class NotificationDialog : StackLayout
    {
        public event EventHandler<EventArgs> Opened;
        public event EventHandler Timeout;

        public NotificationDialog(String i_Message)
        {
            initializeComponent(i_Message);
        }

        protected virtual void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (Opened != null)
            {
                Opened(this, new EventArgs());
            }
        }

        protected void onTimeout(EventArgs e)
        {
            if(Timeout != null)
            {
                Timeout(this, e);
            }
        }

        protected void onOpened(EventArgs e)
        {
            if(Opened != null)
            {
                Opened(this, e);
            }
        }
    }
}
