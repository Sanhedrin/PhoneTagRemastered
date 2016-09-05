using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using PhoneTag.XamarinForms.Controls.SocialMenu;
using PhoneTag.XamarinForms.Controls.AnimatedImageControl;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class LoadingPage : TrailableContentPage
    {
        private void initializeComponent()
        {
            BackgroundColor = Color.Black;

            Title = "Loading";
            Padding = new Thickness(0, 20, 0, 0);
            BackgroundColor = Color.Black;
            Content = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Center
                },
                Children = {
                    new AnimatedImage()
                    {
                        ImageName = "loading_logo",
                        Animate = true,
                        AnimationFrames = 30
                    },
                    new Label
                    {
                        Text = "Loading...",
                        TextColor = Color.White,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            };
        }
    }
}
