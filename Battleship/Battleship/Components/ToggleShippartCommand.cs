using System.Windows.Input;

namespace Battleship.Components
{
    internal class ToggleShippartCommand : BaseCommand
    {
        private readonly PlayfieldViewModel viewModel;

        public ToggleShippartCommand(
            PlayfieldViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override void Execute(object? parameter)
        {
            
            if(parameter is string coordinate)
            {
                char x = coordinate[0];
                char y = coordinate[1];
                viewModel.ToggleShippartAt(x, y);
            }
        }
    }
}