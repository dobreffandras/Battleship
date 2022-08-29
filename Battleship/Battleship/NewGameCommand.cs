using Battleship.Services;
using System;
using System.Windows.Input;

namespace Battleship
{
    internal class NewGameCommand : BaseCommand
    {
        private readonly CommunicationService communicationService;

        public NewGameCommand(CommunicationService communicationService)
        {
            this.communicationService = communicationService;
        }

        public override void Execute(object? parameter)
        {
            communicationService.StartNewGame();
        }
    }
}