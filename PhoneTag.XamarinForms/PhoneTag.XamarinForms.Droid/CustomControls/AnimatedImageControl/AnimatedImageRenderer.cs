using Android.Graphics.Drawables;
using PhoneTag.XamarinForms.Controls.AnimatedImageControl;
using PhoneTag.XamarinForms.Droid.CustomControls.AnimatedImageControl;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AnimatedImage), typeof(AnimatedImageRenderer))]
namespace PhoneTag.XamarinForms.Droid.CustomControls.AnimatedImageControl
{
    public class AnimatedImageRenderer : ImageRenderer
    {
        public AnimatedImageRenderer() { }

        private string m_ImageName;

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                AnimatedImage animatedImage = (AnimatedImage)e.NewElement;

                if (!String.IsNullOrEmpty(animatedImage.ImageName) && animatedImage.Animate)
                {
                    //Setup the animation.
                    Control.ImageAlpha = 0;
                    m_ImageName = animatedImage.ImageName;
                    int imageId = Resources.GetIdentifier(m_ImageName, "drawable", "com.Sanhedrin.PhoneTag");
                    Control.SetBackgroundResource(imageId);

                    (Control.Background as AnimationDrawable)?.Start();
                }
            }
        }
    }
}