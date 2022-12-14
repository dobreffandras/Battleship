using Battleship.Services;

namespace Battleship.Commands
{
    internal class ShootCommand : BaseCommand
    {
        private readonly (char x, char y) coords;
        private readonly CommunicationService communicationService;

        public ShootCommand(
            (char x, char y) coords, 
            CommunicationService communicationService)
        {
            this.coords = coords;
            this.communicationService = communicationService;
        }

        public override bool CanExecute(object? parameter) => true; // TODO consider other players turn

        public override void Execute(object? parameter)
        {
            communicationService.Shoot(coords);
        }
    }
}