using PhoneTag.XamarinForms.Controls.AnimatedImageControl;
using PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.SocialMenu
{
    public abstract partial class PlayerListDisplay : ScrollView
    {
        private void initializeLoadingComponent()
        {
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
                        Animate = true
                    },
                    new Label
                    {
                        Text = "Loading...",
                        TextColor = Color.Black,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            };
        }

        protected virtual async Task initializeComponent(PlayerDetailsTileType i_DetailType)
        {
            StackLayout friendList = await generatePlayerListPresenter(i_DetailType);

            Content = new StackLayout
            {
                Children = {
                    friendList
                }
            };
        }
    }
}
