using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using PhoneTag.XamarinForms.Controls.SocialMenu;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class LoadingPage : TrailableContentPage
    {
        private void initializeComponent()
        {
            Title = "Loading";
            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Center
                },
                Children = {
                    new Label
                    {
                        Text = "Loading..."
                    }
                }
            };
        }
    }
}
