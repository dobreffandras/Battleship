using Battleship.Model;
using System;
using System.ComponentModel;

namespace Battleship.Commands
{
    internal class NewGameCommand : BaseCommand
    {
        private readonly LobbyViewModel lobby;
        private readonly Action<GameModel> navigate;

        public NewGameCommand(
            LobbyViewModel lobby,
            Action<GameModel> navigate)
        {
            this.lobby = lobby;
            this.navigate = navigate;

            lobby.NewGamePlayField.PropertyChanged += NewGamePlayField_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
            => lobby.NewGamePlayField.IsPrepared
            && base.CanExecute(parameter);

        public override void Execute(object? parameter)
        {
            navigate(lobby.CreateNewGame());
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