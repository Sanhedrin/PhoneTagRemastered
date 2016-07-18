using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Views
{
    /// <summary>
    /// Views implementing this interface support updating their stored info to current server values 
    /// instead of requiring a new instance to be requested.
    /// </summary>
    interface IUpdateable
    {
        Task Update();
    }
}
