using PhoneTag.SharedCodebase.Events.GameEvents;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// This page comes up to show the player that was just shot and process the kill.
    /// </summary>
    public partial class ShotDisplayDialog : RelativeLayout
    {
        public static byte[] LastKillCam { get; private set; }
        public event EventHandler ShotCancelled;

        /// <summary>
        /// Initializes the page with the given shot information.
        /// </summary>
        /// <param name="i_ShotPictureBuffer">The picture that was taken of the shot player.</param>
        public ShotDisplayDialog(byte[] i_ShotPictureBuffer)
        {
            LastKillCam = i_ShotPictureBuffer;

            initializeComponent();

            m_ShotView.Source = ImageSource.FromStream(() => new MemoryStream(i_ShotPictureBuffer));
            m_ShotView.RelRotateTo(90);
        }

        private void CancelShotButton_Clicked(object sender, EventArgs e)
        {
            if (ShotCancelled != null)
            {
                ShotCancelled(this, new EventArgs());
            }
        }
    }
}
