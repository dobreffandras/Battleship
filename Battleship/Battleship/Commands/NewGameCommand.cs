using Battleship.Services;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Battleship.Commands
{
    internal class NewGameCommand : BaseCommand
    {
        private readonly LobbyViewModel lobby;
        private readonly CommunicationService communicationService;
        private readonly Action navigate;

        public NewGameCommand(
            LobbyViewModel lobby,
            CommunicationService communicationService,
            Action navigate)
        {
            this.lobby = lobby;
            this.communicationService = communicationService;
            this.navigate = navigate;

            lobby.NewGamePlayField.PropertyChanged += NewGamePlayField_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
            => lobby.NewGamePlayField.IsPrepared
            && base.CanExecute(parameter);

        public override void Execute(object? parameter)
        {
            communicationService.StartNewGame();
            navigate();
        }

        private void NewGamePlayField_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Components.PreparingPlayfieldViewModel.IsPrepared))
            {
                RaiseCanExecuteChanged();
            }
        }
    }
}