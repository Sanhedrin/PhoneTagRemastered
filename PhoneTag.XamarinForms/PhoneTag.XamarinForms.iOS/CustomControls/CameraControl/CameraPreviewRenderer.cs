using System;
using PhoneTag.XamarinForms;
using PhoneTag.XamarinForms.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using AVFoundation;
using System.IO;
using System.Threading.Tasks;
using CoreMedia;
using Foundation;
using PhoneTag.XamarinForms.Controls.CameraControl;
using PhoneTag.XamarinForms.iOS.CustomControls.CameraControl;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraPreviewRenderer))]
namespace PhoneTag.XamarinForms.iOS.CustomControls.CameraControl
{
    public class CameraPreviewRenderer : ViewRenderer<CameraPreview, UICameraPreview>
    {
        UICameraPreview uiCameraPreview;
        AVCaptureStillImageOutput output;

        protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                uiCameraPreview = new UICameraPreview(e.NewElement.Camera);
                SetNativeControl(uiCameraPreview);
            }
            if (e.OldElement != null)
            {
                e.OldElement.PictureRequested -= OnTakePictureRequested;
            }
            if (e.NewElement != null)
            {
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
            i_Callback(pictureStream);
        }

        /// <summary>
        /// Takes the picture using the Android's platform camera interface.
        /// </summary>
        /// <returns>Byte array representing the picture that was just taken.</returns>
        public async Task<byte[]> TakePicture()
        {
            //Might need this if CaptureSession.Outputs is empty, can't tell without testing
            //output = new AVCaptureStillImageOutput
            //{
            //    OutputSettings = new Foundation.NSDictionary(AVVideo.CodecKey, AVVideo.CodecJPEG)
            //};
            //uiCameraPreview.CaptureSession.AddOutput(output)

            CMSampleBuffer buffer = await ((AVCaptureStillImageOutput)uiCameraPreview.CaptureSession.Outputs[0]).CaptureStillImageTaskAsync(uiCameraPreview.CaptureSession.Outputs[0].Connections[0]);
            NSData data = AVCaptureStillImageOutput.JpegStillToNSData(buffer);

            uiCameraPreview.CaptureSession.StartRunning();

            byte[] bytes = new byte[data.AsStream().Length];
            await data.AsStream().ReadAsync(bytes, 0, (int)data.AsStream().Length);
            return bytes;
        }

        void TriggerCameraPreview()
        {
            if (uiCameraPreview.IsPreviewing)
            {
                uiCameraPreview.CaptureSession.StopRunning();
                uiCameraPreview.IsPreviewing = false;
            }
            else {
                uiCameraPreview.CaptureSession.StartRunning();
                uiCameraPreview.IsPreviewing = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Control.CaptureSession.Dispose();
                Control.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}