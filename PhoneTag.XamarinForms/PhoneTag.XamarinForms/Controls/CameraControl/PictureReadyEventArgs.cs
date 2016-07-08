using System;

namespace PhoneTag.XamarinForms.Controls.CameraControl
{
    public class PictureReadyEventArgs : EventArgs
    {
        /// <summary>
        /// A byte array representing the picture that was just taken.
        /// </summary>
        public byte[] PictureBuffer { get; set; }
    }
}