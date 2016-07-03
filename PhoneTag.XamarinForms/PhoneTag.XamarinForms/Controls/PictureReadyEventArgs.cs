using System;

namespace PhoneTag.XamarinForms.Controls
{
    public class PictureReadyEventArgs : EventArgs
    {
        public byte[] PictureBuffer { get; set; }
    }
}