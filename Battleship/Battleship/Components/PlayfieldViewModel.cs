using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Battleship.Components
{
    internal class PlayfieldViewModel
    {

        public PlayfieldViewModel(PlayfieldModel model)
        {
            ToggleShippart = new ToggleShippartCommand(model);
        }
        public ICommand ToggleShippart { get; set; }
    }
}
