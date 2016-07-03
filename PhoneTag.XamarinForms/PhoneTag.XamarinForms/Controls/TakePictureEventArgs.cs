using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.XamarinForms.Controls
{
    public class TakePictureEventArgs : EventArgs
    {
        public Action<byte[]> PictureReadyCallback { get; set; }
    }
}
