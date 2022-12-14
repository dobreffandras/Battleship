﻿using Battleship.Components;
using Battleship.Services;

namespace Battleship
{
    internal class GameViewModel : BaseViewModel
    {
        private readonly CommunicationService communicationService;

        public GameViewModel(PlayfieldModel model, CommunicationService communicationService)
        {
            this.communicationService = communicationService;
            communicationService.GameActionCallback = ChangeMessageReceived;

            PlayingFieldViewModel = new PlayingFieldViewModel(model, communicationService);
        }

        public PlayingFieldViewModel PlayingFieldViewModel { get; }

        public void ChangeMessageReceived(GameMessage message)
        {
            PlayingFieldViewModel.ShootOn(message.X, message.Y);
            NotifyPropertyChanged(nameof(PlayingFieldViewModel));
        }
    }
}