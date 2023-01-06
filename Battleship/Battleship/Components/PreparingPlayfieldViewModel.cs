using Battleship.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Battleship.Components
{
    internal class PreparingPlayfieldViewModel : BaseViewModel
    {
        private readonly PlayfieldModel model;

        public PreparingPlayfieldViewModel(PlayfieldModel model)
        {
            ToggleShippart = new ToggleShippartCommand(this);
            this.model = model;
        }

        public ICommand ToggleShippart { get; set; }

        public IDictionary<string, string> Shipparts =>
            model.Shipparts.ToDictionary(
                kv => $"{kv.Key.Item1}{kv.Key.Item2}",
                kv => kv.Value.ToString());

        public bool IsPrepared => model.IsPrepared;

        public PlayfieldModel Model => model;

        internal void ToggleShippartAt(char x, char y)
        {
            model.ToggleShippart(x, y);
            NotifyPropertyChanged(nameof(IsPrepared));
            NotifyPropertyChanged(nameof(Shipparts));
        }
    }
}
