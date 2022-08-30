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

        public bool IsPrepared => model.IsPrepared;

        public PlayfieldModel Model => model;

        internal void ToggleShippartAt(char x, char y)
        {
            model.ToggleShippart(x, y);
            NotifyPropertyChanged(nameof(IsPrepared));
        }
    }
}
