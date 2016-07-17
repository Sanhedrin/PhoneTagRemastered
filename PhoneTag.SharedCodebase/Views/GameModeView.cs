using PhoneTag.WebServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.WebServices.Views
{
    public abstract class GameModeView
    {
        public String Name { get; set; }
        public abstract int TotalNumberOfPlayers { get; }

        public GameModeView()
        {
            Name = GameModeFactory.GetNameOfModeViewType(this.GetType());
        }
    }
}
