using System;
using Android.Hardware;
using PhoneTag.XamarinForms;
using PhoneTag.XamarinForms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.IO;
using System.Threading.Tasks;
using PhoneTag.XamarinForms.Controls;
using PhoneTag.XamarinForms.Droid.CustomControls.CameraControl;

[assembly: ExportRenderer(typeof(PhoneTag.XamarinForms.Controls.CameraPreview), typeof(CameraPreviewRenderer))]
namespace PhoneTag.XamarinForms.Droid.CustomControls.CameraControl
{
    public class CameraPreviewRenderer : ViewRenderer<PhoneTag.XamarinForms.Controls.CameraPreview, PhoneTag.XamarinForms.Droid.CustomControls.CameraControl.UICameraPreview>, Camera.IPictureCallback
    {
        UICameraPreview cameraPreview;

        byte[] m_PictureDataStream = null;

        protected override void OnElementChanged(ElementChangedEventArgs<PhoneTag.XamarinForms.Controls.CameraPreview> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                cameraPreview = new UICameraPreview(Context);
                SetNativeControl(cameraPreview);
            }

            if (e.OldElement != null)
            {
                e.OldElement.PictureRequested -= OnTakePictureRequested;
            }
            if (e.NewElement != null)
            {
                Control.Preview = Camera.Open((int)e.NewElement.Camera);
                e.NewElement.PictureRequested += OnTakePictureRequested;
            }
        }

        private async void OnTakePictureRequested(object sender, TakePictureEventArgs e)
        {
            byte[] pictureStream = await TakePicture();
            e.PictureReadyCallback(pictureStream);
        }

        public async Task<byte[]> TakePicture()
        {
            m_PictureDataStream = null;
            cameraPreview.Preview.TakePicture(null, null, null, this);

            await Task.Run(() => { while (m_PictureDataStream == null) ; });

            cameraPreview.Preview.StartPreview();

            return m_PictureDataStream;
        }

        public void OnPictureTaken(byte[] data, Camera camera)
        {
            m_PictureDataStream = data;
        }

        public void ToggleCameraPreview()
        {
            if (cameraPreview.IsPreviewing)
            {
                cameraPreview.Preview.StopPreview();
                cameraPreview.IsPreviewing = false;
            }
            else {
                cameraPreview.Preview.StartPreview();
                cameraPreview.IsPreviewing = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Control.Preview.Release();
            }
            base.Dispose(disposing);
        }
    }
}