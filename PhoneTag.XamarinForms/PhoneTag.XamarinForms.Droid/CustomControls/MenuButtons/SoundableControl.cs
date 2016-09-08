using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PhoneTag.XamarinForms.Droid.CustomControls.MenuButtons;
using PhoneTag.XamarinForms.Controls.MenuButtons;
using Plugin.CurrentActivity;
using Android.Media;

[assembly: Xamarin.Forms.Dependency(typeof(SoundableControl))]
namespace PhoneTag.XamarinForms.Droid.CustomControls.MenuButtons
{
    public class SoundableControl : ISound
    {
        private View m_Root;

        public void ButtonClick()
        {
            if (m_Root == null)
            {
                m_Root = CrossCurrentActivity.Current.Activity.FindViewById<View>(Android.Resource.Id.Content);
            }
            m_Root.PlaySoundEffect(SoundEffects.Click);
        }

        public void PlayBam()
        {
            MediaPlayer player = new MediaPlayer();

            player.Reset();
            //player.SetDataSource();
            player.Prepare();
            player.Start();
        }
    }
}