using PhoneTag.XamarinForms.Controls.MenuButtons;
using PhoneTag.XamarinForms.iOS.CustomControls.MenuButtons;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(SoundableControl))]
namespace PhoneTag.XamarinForms.iOS.CustomControls.MenuButtons
{
    public class SoundableControl : ISound
    {
        public void ButtonClick()
        {
            UIDevice.CurrentDevice.PlayInputClick();
        }

        public void PlayBam()
        {
            throw new NotImplementedException();
        }
    }
}
