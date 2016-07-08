using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.WebServices.Models
{
    /// <summary>
    /// Model classes implementing this must support a way to view them.
    /// </summary>
    interface IViewable
    {
        /// <summary>
        /// Generates a view into this model.
        /// </summary>
        /// <returns>The view class of this model as a dynamic.</returns>
        dynamic GenerateView();
    }
}
