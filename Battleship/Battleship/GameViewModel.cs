﻿using Battleship.Components;
using Battleship.Model;
using Battleship.Services;

namespace Battleship
{
    internal class GameViewModel : BaseViewModel
    {
        public GameViewModel(GameModel game, CommunicationService communicationService)
        {
            // TODO Consider collecting these under an interface
            communicationService.ShootCallback = ShootMessageReceived;
            communicationService.ShootResponseCallback = ShootResponseMessageReceived;
            communicationService.OpponentConnectedCallback = OpponentConnected;
            communicationService.ConnectionAcceptedCallback = ConnectionAccepted;
            communicationService.OpponentLeftCallback = OpponentLeft;

            this.game = game;
            this.communicationService = communicationService;
            MyPlayingFieldViewModel = new PlayingFieldViewModel(game.MyPlayfieldModel, PlayingType.Passive, communicationService);
            OtherPlayingFieldViewModel = new PlayingFieldViewModel(game.OtherPlayfieldModel, PlayingType.Active, communicationService);
        }

        private readonly GameModel game;
        private readonly CommunicationService communicationService;

        public string GameId => game.GameId;

        public string PlayerSide => game.Player == Player.PlayerOne ? "Player 1" : "Player 2";

        public PlayingFieldViewModel MyPlayingFieldViewModel { get; }

        public PlayingFieldViewModel OtherPlayingFieldViewModel { get; }

        public string GameState { get => game.State.Text; }

        public void OpponentConnected()
        {
            game.OpponentConnected();
            communicationService.AcceptConnection();
            NotifyPropertyChanged(nameof(GameState));
        }
        public void ConnectionAccepted()
        {
            game.ConnectionAccepted();
            NotifyPropertyChanged(nameof(GameState));
        }

        public void OpponentLeft()
        {
            game.OpponentLeft();
            NotifyPropertyChanged(nameof(GameState));
        }

        public void ShootMessageReceived(ShootMessage message)
        {
            var x = message.X;
            var y = message.Y;

            var (isShippart, shootState) = game.ReceiveShoot(x, y);
            communicationService.Respond((x, y), isShippart, shootState);
            NotifyPropertyChanged(nameof(GameState));
            MyPlayingFieldViewModel.NotifyAllPropertiesChanged();
        }

        public void ShootResponseMessageReceived(ShootResponseMessage message)
        {
            game.ReceiveResponseForShoot(message.X, message.Y, message.IsShippart, message.ShootState);
            NotifyPropertyChanged(nameof(GameState));
            OtherPlayingFieldViewModel.NotifyAllPropertiesChanged();
        }
    }
}