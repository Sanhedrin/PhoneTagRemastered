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
    public partial class ShotDisplayDialog : RelativeLayout
    {
        private Image m_ShotView;

        private async Task initializeComponent()
        {
            BackgroundColor = Color.Black;

            m_ShotView = generateShotImage();
            ShotSuggestionListDisplay shotSuggestions = generatePlayerShotSuggestions();
            Button cancelShotButton = generateCancelShotButton();

            Children.Add(cancelShotButton,
                xConstraint: Constraint.RelativeToParent((parent) => { return 10; }),
                yConstraint: Constraint.RelativeToParent((parent) => { return 10; }),
                heightConstraint: Constraint.RelativeToParent((parent) => { return parent.Height/ 20; }));

            Children.Add(m_ShotView,
                xConstraint: Constraint.RelativeToParent((parent) => { return 0; }),
                yConstraint: Constraint.RelativeToParent((parent) => { return parent.Height / 5; }));

            Children.Add(shotSuggestions,
                widthConstraint: Constraint.RelativeToParent((parent) => { return parent.Width; }),
                yConstraint: Constraint.RelativeToParent((parent) => { return parent.Height - shotSuggestions.HeightRequest; }));
        }

        private Button generateCancelShotButton()
        {
            Button cancelShotButton = new Button();
            
            cancelShotButton.Text = "Cancel";
            cancelShotButton.BackgroundColor = Color.Gray;
            cancelShotButton.Clicked += CancelShotButton_Clicked;

            return cancelShotButton;
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
