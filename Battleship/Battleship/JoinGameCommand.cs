using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Battleship
{
    internal class JoinGameCommand : ICommand
    {
        private readonly LobbyViewModel lobby;

        public JoinGameCommand(LobbyViewModel lobby)
        {
            this.lobby = lobby;

            lobby.PropertyChanged += Lobby_PropertyChanged;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => lobby.SelectedGameIndex > 0;

        public void Execute(object? parameter)
        {
            var selected = this.lobby.SelectedGameItem as string;
            MessageBox.Show(selected);
        }

        private void Lobby_PropertyChanged(
            object? sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(LobbyViewModel.SelectedGameIndex))
            {
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }
    }
}