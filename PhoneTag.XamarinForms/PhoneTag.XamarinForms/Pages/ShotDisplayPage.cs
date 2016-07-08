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
    public partial class ShotDisplayPage : ContentPage
    {
        /// <summary>
        /// Initializes the page with the given shot information.
        /// </summary>
        /// <param name="i_ShotPictureBuffer">The picture that was taken of the shot player.</param>
        public ShotDisplayPage(byte[] i_ShotPictureBuffer)
        {
            initializeComponent();

            m_ShotView.Source = ImageSource.FromStream(() => new MemoryStream(i_ShotPictureBuffer));
            m_ShotView.RelRotateTo(90);
            Padding = new Thickness(0, CrossScreen.Current.Size.Height * 1 / 4, 0, 0);
        }
    }
}
