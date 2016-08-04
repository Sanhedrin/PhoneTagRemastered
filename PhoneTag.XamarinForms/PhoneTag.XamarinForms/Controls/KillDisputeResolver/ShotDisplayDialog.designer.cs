using PhoneTag.XamarinForms.Controls.MenuButtons;
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

            AbsoluteLayout killcamLayout = generateKillcamLayout();
            ShotSuggestionListDisplay shotSuggestions = generatePlayerShotSuggestions();
            ImageButton cancelShotButton = generateCancelShotButton();

            Children.Add(cancelShotButton,
                Constraint.RelativeToParent((parent) => { return 0; }),
                Constraint.RelativeToParent((parent) => { return -parent.Height / 6; }),
                Constraint.RelativeToParent((parent) => { return parent.Width / 8; }));
            Children.Add(killcamLayout,
                Constraint.RelativeToParent((parent) => { return parent.Width / 5; }),
                Constraint.RelativeToParent((parent) => { return parent.Height / 2; }));
            Children.Add(shotSuggestions,
                widthConstraint: Constraint.RelativeToParent((parent) => { return parent.Width; }),
                yConstraint: Constraint.RelativeToParent((parent) => { return parent.Height - shotSuggestions.HeightRequest; }));
        }

        private AbsoluteLayout generateKillcamLayout()
        {
            AbsoluteLayout killcamLayout = new AbsoluteLayout();

            Image killcamFrameImage = generateKillcamFrameImage();
            AbsoluteLayout.SetLayoutFlags(killcamFrameImage, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(killcamFrameImage, new Rectangle(0.5, 1.85, 1.5, 1.5));

            m_ShotView = generateShotImage();
            AbsoluteLayout.SetLayoutBounds(m_ShotView, new Rectangle(0.5, -2.88, 1.71, 0.8));
            AbsoluteLayout.SetLayoutFlags(m_ShotView, AbsoluteLayoutFlags.All);

            killcamLayout.Children.Add(m_ShotView);
            killcamLayout.Children.Add(killcamFrameImage);

            return killcamLayout;
        }

        private Image generateKillcamFrameImage()
        {
            Image killcamFrame = new Image()
            {
                Source = ImageSource.FromFile("killcam_frame.png")
            };

            return killcamFrame;
        }

        private ImageButton generateCancelShotButton()
        {
            ImageButton cancelShotButton = new ImageButton();

            cancelShotButton.Source = "back_button.png";
            cancelShotButton.ClickAction = () => { CancelShotButton_Clicked(); };

            return cancelShotButton;
        }

        private ShotSuggestionListDisplay generatePlayerShotSuggestions()
        {
            ShotSuggestionListDisplay container = new ShotSuggestionListDisplay();

            container.HeightRequest = CrossScreen.Current.Size.Height * 1 / 4;

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
