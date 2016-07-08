using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

/// <summary>
/// A custom camera control that can be used as a view on a page to show built-in camera's view and allow
/// to take pictures with it.
/// </summary>
namespace PhoneTag.XamarinForms.Controls.CameraControl
{
    public class CameraPreview : View
    {
        public event EventHandler<TakePictureEventArgs> PictureRequested;
        public event EventHandler<PictureReadyEventArgs> PictureReady;

        public static readonly BindableProperty CameraProperty = BindableProperty.Create(
            propertyName: "Camera",
            returnType: typeof(CameraOptions),
            declaringType: typeof(CameraPreview),
            defaultValue: CameraOptions.Rear);

        public CameraPreview() : base()
        {
        }

        public CameraOptions Camera
        {
            get { return (CameraOptions)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }

        /// <summary>
        /// Takes a picture and calls the OnPictureReady method once processing is complete.
        /// </summary>
        //Note: Taking pictures is a platform specific operation.
        //The underlying custom renderer listens to this event to know when a picture request is made,
        //and then once it's done taking the picture calls back the onready method given to it.
        public void TakePicture()
        {
            if (PictureRequested != null)
            {
                PictureRequested(this, new TakePictureEventArgs { PictureReadyCallback = OnPictureReady });
            }
        }

        /// <summary>
        /// Called once a picture has been taken and fires off the event to any listeners interested in the 
        /// acquired picture.
        /// </summary>
        /// <param name="i_PictureBuffer">The byte array represnting the picture that was taken.</param>
        private void OnPictureReady(byte[] i_PictureBuffer)
        {
            if(PictureReady != null)
            {
                PictureReady(this, new PictureReadyEventArgs { PictureBuffer = i_PictureBuffer });
            }
        }
    }
}
