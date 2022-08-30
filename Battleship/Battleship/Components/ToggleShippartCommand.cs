using System.Windows.Input;

namespace Battleship.Components
{
    internal class ToggleShippartCommand : BaseCommand
    {
        private readonly PlayfieldModel playfieldModel;

        public ToggleShippartCommand(PlayfieldModel playfieldModel)
        {
            this.playfieldModel = playfieldModel;
        }

        public override void Execute(object? parameter)
        {
            if(parameter is string coordinate)
            {
                char x = coordinate[0];
                char y = coordinate[1];

                playfieldModel[(x, y)] = !playfieldModel[(x, y)];
            }
        }
    }
}