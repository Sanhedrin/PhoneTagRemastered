using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class ShotDisplayPage
    {
        private Image m_ShotView;

        private void initializeComponent()
        {
            m_ShotView = new Image
            {
                Aspect = Aspect.Fill,
                HeightRequest = CrossScreen.Current.Size.Height * 2 / 5,
                WidthRequest = CrossScreen.Current.Size.Width,
            };

            Title = "Main Page";
            Content = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                Children = {
                    m_ShotView
                }
            };
        }
    }
}
