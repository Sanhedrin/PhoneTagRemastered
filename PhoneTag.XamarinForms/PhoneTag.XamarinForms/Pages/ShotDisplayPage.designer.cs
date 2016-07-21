using PhoneTag.XamarinForms.Controls.SocialMenu;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class ShotDisplayPage : TrailableContentPage
    {
        private Image m_ShotView;

        private async Task initializeComponent()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            m_ShotView = generateShotImage();
            ShotSuggestionListDisplay shotSuggestions = generatePlayerShotSuggestions();

            Title = "Main Page";
            Content = new RelativeLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                },
                Children = {
                }
            };

            (Content as RelativeLayout).Children.Add(m_ShotView,
                Constraint.RelativeToParent((parent) => { return 0; }));

            (Content as RelativeLayout).Children.Add(shotSuggestions,
                widthConstraint: Constraint.RelativeToParent((parent) => { return parent.Width; }),
                yConstraint: Constraint.RelativeToParent((parent) => { return parent.Height - shotSuggestions.HeightRequest; }));
        }

        private ShotSuggestionListDisplay generatePlayerShotSuggestions()
        {
            ShotSuggestionListDisplay container = new ShotSuggestionListDisplay();

            container.HeightRequest = CrossScreen.Current.Size.Height / 6;

            return container;
        }

        private Image generateShotImage()
        {
            return new Image
            {
                Aspect = Aspect.Fill,
                HeightRequest = CrossScreen.Current.Size.Height * 2 / 5,
                WidthRequest = CrossScreen.Current.Size.Width,
            };
        }
    }
}
