using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.AnimatedImageControl
{
    public class AnimatedImage : Image
    {
        public static readonly BindableProperty AnimateProperty = BindableProperty.Create(
            propertyName: "Animate",
            returnType: typeof(bool),
            declaringType: typeof(AnimatedImage),
            defaultValue: false);

        /// <summary>
        /// Should the image animate?
        /// </summary>
        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        /// <summary>
        /// iOS ONLY(Android animation is defined in the resources as xml)
        /// Determines the duration in seconds it takes to complete 1 animation cycle.
        /// </summary>
        public double AnimationDuration { get; set; }

        /// <summary>
        /// iOS ONLY(Android animation is defined in the resources as xml)
        /// How many frames are in this animation.
        /// </summary>
        public int AnimationFrames { get; set; }

        /// <summary>
        /// The name of the image file to load.
        /// </summary>
        public String ImageName { get; set; }

        public AnimatedImage()
        {

        }

        /// <summary>
        /// Initializes a new animated image.
        /// </summary>
        /// <param name="i_ImageName">The name of the image file to use, where for image whose name is x, each frame is marked x_1, x_2, etc. in order of frames
        /// iOS: Place the images under the Resources folder.
        /// Android: Place the images under the Resources/drawable</param>
        /// <param name="i_NumberOfFrames">iOS Only: Number of frames in this animation, should match the number of files matching the name format.</param>
        /// <param name="i_AnimationCycleDuration">iOS Only: Duration in seconds for an animation cycle to complete.</param>
        public AnimatedImage(String i_ImageName, int i_NumberOfFrames = 0, double i_AnimationCycleDuration = 1)
        {
            ImageName = i_ImageName;
            AnimationDuration = i_AnimationCycleDuration;
            AnimationFrames = i_NumberOfFrames;
            Animate = true;
        }
    }
}
