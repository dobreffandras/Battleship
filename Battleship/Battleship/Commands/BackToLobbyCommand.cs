using Battleship.Services;
using System;

namespace Battleship.Commands
{
    internal class BackToLobbyCommand : BaseCommand
    {
        private readonly Action navigate;
        private readonly GameViewModel gameViewModel;

        public BackToLobbyCommand(GameViewModel gameViewModel, NavigationService navigationService)
        {
            this.navigate = navigationService.ToLobbyViewModel;
            this.gameViewModel = gameViewModel;
        }

        public override void Execute(object? parameter)
        {
            gameViewModel.LeaveGame();
            navigate();
        }
    }
}
