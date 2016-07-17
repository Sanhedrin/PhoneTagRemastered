using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.WebServices.Views
{
    /// <summary>
    /// This is a POCO class that holds the user's social information.
    /// Info is gathered from the user's FB account, and therefore doesn't require a model on our part.
    /// </summary>
    public class UserSocialView
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String ProfilePictureUrl { get; set; }
    }
}
