using Battleship.Services;
using System;

namespace Battleship.Commands
{
    internal class BackToLobbyCommand : BaseCommand
    {
        private readonly Action navigate;

        public BackToLobbyCommand(NavigationService navigationService)
        {
            this.navigate = navigationService.ToLobbyViewModel;
        }

        public override void Execute(object? parameter)
        {
            navigate();
        }
    }
}
