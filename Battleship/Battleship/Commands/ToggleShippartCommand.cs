using System;
namespace Battleship.Commands
{
    internal class ToggleShippartCommand : BaseCommand
    {
        private readonly Action<char, char> toggleAction;

        public ToggleShippartCommand(Action<char, char> toggleAction)
        {
            this.toggleAction = toggleAction;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is string coordinate)
            {
                char x = coordinate[0];
                char y = coordinate[1];
                toggleAction(x, y);
            }
        }
    }
}