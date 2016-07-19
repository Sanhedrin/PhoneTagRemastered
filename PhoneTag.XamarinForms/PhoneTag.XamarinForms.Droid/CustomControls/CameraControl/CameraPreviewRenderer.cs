using System;
using Android.Hardware;
using PhoneTag.XamarinForms;
using PhoneTag.XamarinForms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.IO;
using System.Threading.Tasks;
using PhoneTag.XamarinForms.Controls.CameraControl;
using PhoneTag.XamarinForms.Droid.CustomControls.CameraControl;

[assembly: ExportRenderer(typeof(PhoneTag.XamarinForms.Controls.CameraControl.CameraPreview), typeof(CameraPreviewRenderer))]
namespace PhoneTag.XamarinForms.Droid.CustomControls.CameraControl
{
    /// <summary>
    /// The renderer for the custom camera control.
    /// </summary>
    public class CameraPreviewRenderer : ViewRenderer<PhoneTag.XamarinForms.Controls.CameraControl.CameraPreview, PhoneTag.XamarinForms.Droid.CustomControls.CameraControl.UICameraPreview>, Camera.IPictureCallback
    {
        UICameraPreview cameraPreview;

        byte[] m_PictureDataStream = null;

        protected override void OnElementChanged(ElementChangedEventArgs<PhoneTag.XamarinForms.Controls.CameraControl.CameraPreview> e)
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

        //Handles a picture request.
        private void OnTakePictureRequested(object sender, TakePictureEventArgs e)
        {
            takePicture(e.PictureReadyCallback);
        }

        private async Task takePicture(Action<byte[]> i_Callback)
        {
            byte[] pictureStream = await TakePicture();

            if (pictureStream != null)
            {
                i_Callback(pictureStream);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR!");
                System.Diagnostics.Debug.WriteLine("Camera: Picture taking failed");
            }
        }

        /// <summary>
        /// Takes the picture using the Android's platform camera interface.
        /// </summary>
        /// <returns>Byte array representing the picture that was just taken.</returns>
        public async Task<byte[]> TakePicture()
        {
            m_PictureDataStream = null;

            try
            {
                cameraPreview.Preview.TakePicture(null, null, null, this);

                await Task.Run(() => { while (m_PictureDataStream == null) ; });

                cameraPreview.Preview.StartPreview();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION!");
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return m_PictureDataStream;
        }

        /// <summary>
        /// Called once the picture is taken.
        /// </summary>
        /// <param name="data">Byte array representing the picture that was just taken.</param>
        /// <param name="camera">The camera that took the picture.</param>
        public void OnPictureTaken(byte[] data, Camera camera)
        {
            m_PictureDataStream = data;
        }

        /// <summary>
        /// Enables the streaming of the physical camera to the view.
        /// </summary>
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