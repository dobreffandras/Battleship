using Battleship.Services;
using System;
using System.Windows.Input;

namespace Battleship
{
    internal class NewGameCommand : BaseCommand
    {
        private readonly CommunicationService communicationService;
        private readonly Action navigate;

        public NewGameCommand(
            CommunicationService communicationService,
            Action navigate)
        {
            this.communicationService = communicationService;
            this.navigate = navigate;
        }

        public override void Execute(object? parameter)
        {
            communicationService.StartNewGame();
            navigate();
        }
    }
}