using Battleship.Model;
using System;

namespace Battleship.Services
{
    internal class NavigationService
    {
        private readonly Action<BaseViewModel> navigate;
        private readonly CommunicationService communicationService;

        public NavigationService(
            Action<BaseViewModel> navigationAction, 
            CommunicationService communicationService)
        {
            navigate = navigationAction;
            this.communicationService = communicationService;
        }

        internal void ToGameViewModel(GameModel gameModel) 
            => navigate(new GameViewModel(gameModel, communicationService, this));

        internal void ToLobbyViewModel()
            => navigate(new LobbyViewModel(communicationService, this));

    }
}
