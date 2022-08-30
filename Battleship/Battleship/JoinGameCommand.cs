using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Battleship
{
    internal class JoinGameCommand : BaseCommand
    {
        private readonly LobbyViewModel lobby;
        private readonly Action navigate;

        public JoinGameCommand(LobbyViewModel lobby, Action navigate)
        {
            this.lobby = lobby;
            this.navigate = navigate;
            lobby.PropertyChanged += Lobby_PropertyChanged;
        }

        public override bool CanExecute(object? parameter) 
            => lobby.SelectedGameIndex >= 0 && base.CanExecute(parameter);

        public override void Execute(object? parameter)
        {
            this.lobby.JoinGameAction();
            navigate();
        }

        private void Lobby_PropertyChanged(
            object? sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(LobbyViewModel.SelectedGameIndex))
            {
                RaiseCanExecuteChanged();
            }
        }
    }
}