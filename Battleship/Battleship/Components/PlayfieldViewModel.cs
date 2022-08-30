using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Battleship.Components
{
    internal class PlayfieldViewModel : BaseViewModel
    {
        private readonly PlayfieldModel model;

        public PlayfieldViewModel(PlayfieldModel model)
        {
            ToggleShippart = new ToggleShippartCommand(this);
            this.model = model;
        }
        public ICommand ToggleShippart { get; set; }

        public bool IsPrepared => model.IsPrepared;

        internal void ToggleShippartAt(char x, char y)
        {
            model[(x, y)] = !model[(x, y)];
            NotifyPropertyChanged(nameof(IsPrepared));
        }
    }
}
