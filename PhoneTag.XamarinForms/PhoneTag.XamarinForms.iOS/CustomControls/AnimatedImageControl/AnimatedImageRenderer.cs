using Foundation;
using PhoneTag.XamarinForms.Controls.AnimatedImageControl;
using PhoneTag.XamarinForms.iOS.CustomControls.AnimatedImageControl;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AnimatedImage), typeof(AnimatedImageRenderer))]
namespace PhoneTag.XamarinForms.iOS.CustomControls.AnimatedImageControl
{
    public class AnimatedImageRenderer : ImageRenderer
    {
        private UIImage[] m_ImageArray;
        private double m_AnimationDuration;
        private int m_AnimationFrames;
        private string m_ImageName;

        public AnimatedImageRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                AnimatedImage animatedImage = (AnimatedImage)e.NewElement;

                if (!String.IsNullOrEmpty(animatedImage.ImageName) && animatedImage.Animate)
                {
                    //Setup the animation.
                    m_ImageName = animatedImage.ImageName;
                    m_AnimationDuration = animatedImage.AnimationDuration;
                    m_AnimationFrames = animatedImage.AnimationFrames;

                    //Setup the animation.
                    m_ImageArray = new UIImage[m_AnimationFrames];
                    for (int i = 0; i < m_AnimationFrames; i++)
                    {
                        m_ImageArray[i] = UIImage.FromFile(new NSString($"{m_ImageName}_{i+1}.png"));
                        Control.Image = m_ImageArray[0];
                    }

                    Control.AnimationImages = m_ImageArray;
                    Control.AnimationDuration = m_AnimationDuration;
                    Control.AnimationRepeatCount = 0;

                    Control?.StartAnimating();
                }
            }
        }
    }
}
