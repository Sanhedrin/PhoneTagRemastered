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
    public partial class ShotDisplayPage : ContentPage
    {
        public ShotDisplayPage(byte[] i_ShotPictureBuffer)
        {
            initializeComponent();
            
            NavigationPage.SetHasNavigationBar(this, false);

            m_ShotView.Source = ImageSource.FromStream(() => new MemoryStream(i_ShotPictureBuffer));
            m_ShotView.RelRotateTo(90);
            System.Diagnostics.Debug.WriteLine((CrossScreen.Current.Size.Width * 2 / 5).ToString());
            System.Diagnostics.Debug.WriteLine(CrossScreen.Current.Size.Height.ToString());
            Padding = new Thickness(0, CrossScreen.Current.Size.Height * 1 / 4, 0, 0);
        }
    }
}
