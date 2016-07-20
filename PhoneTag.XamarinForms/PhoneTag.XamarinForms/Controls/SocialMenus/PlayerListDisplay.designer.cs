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
            BackgroundColor = Color.Black;

            Content = new StackLayout
            {
                Children =
                {
                    new Label()
                    {
                        Text = "Loading..."
                    }
                }
            };
        }

        private async void initializeComponent(PlayerDetailsTileType i_DetailType)
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
