using PhoneTag.SharedCodebase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase.Views
{
    public abstract class GameModeView
    {
        public String Name { get; set; }

        public GameModeView()
        {
            Name = GameModeFactory.GetNameOfModeViewType(this.GetType());
        }
    }
}
