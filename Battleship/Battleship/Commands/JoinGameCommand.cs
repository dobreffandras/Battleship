using Battleship.Model;
using System;
using System.ComponentModel;

namespace Battleship.Commands
{
    internal class JoinGameCommand : BaseCommand
    {
        private readonly LobbyViewModel lobby;
        private readonly Action<PlayfieldModel> navigate;

        public JoinGameCommand(LobbyViewModel lobby, Action<PlayfieldModel> navigate)
        {
            this.lobby = lobby;
            this.navigate = navigate;
            lobby.PropertyChanged += Lobby_PropertyChanged;
            lobby.NewGamePlayField.PropertyChanged += Lobby_NewGamePlayField_Propertychanged;
        }

        public override bool CanExecute(object? parameter)
            => lobby.SelectedGameIndex >= 0
            && lobby.NewGamePlayField.IsPrepared
            && base.CanExecute(parameter);

        public override void Execute(object? parameter)
        {
            lobby.JoinGameAction();
            navigate(lobby.NewGamePlayField.Model);
        }

        private void Lobby_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LobbyViewModel.SelectedGameIndex))
            {
                RaiseCanExecuteChanged();
            }
        }
        private void Lobby_NewGamePlayField_Propertychanged(
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