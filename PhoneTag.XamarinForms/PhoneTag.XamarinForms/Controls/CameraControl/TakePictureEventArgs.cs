using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.XamarinForms.Controls.CameraControl
{
    public class TakePictureEventArgs : EventArgs
    {
        /// <summary>
        /// The method to invoke once the picture is taken and pass the picture data to.
        /// </summary>
        public Action<byte[]> PictureReadyCallback { get; set; }
    }
}
