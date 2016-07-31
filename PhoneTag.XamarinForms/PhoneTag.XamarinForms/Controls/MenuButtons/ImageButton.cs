using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.MenuButtons
{
    public class ImageButton : Image
    {
        public Action ClickAction { get; set; }

        public ImageButton()
        {
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            buttonTapped();
        }

        private async Task buttonTapped()
        {
            await this.ScaleTo(0.9, 75, Easing.CubicOut);
            await this.ScaleTo(1, 75, Easing.CubicIn);

            if (ClickAction != null)
            {
                ClickAction();
            }
        }
    }
}
