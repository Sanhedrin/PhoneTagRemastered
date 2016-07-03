using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls
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

        public void TakePicture()
        {
            if (PictureRequested != null)
            {
                PictureRequested(this, new TakePictureEventArgs { PictureReadyCallback = OnPictureReady });
            }
        }

        private void OnPictureReady(byte[] i_PictureBuffer)
        {
            if(PictureReady != null)
            {
                PictureReady(this, new PictureReadyEventArgs { PictureBuffer = i_PictureBuffer });
            }
        }
    }
}
