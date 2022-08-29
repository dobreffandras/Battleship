﻿using Battleship.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Battleship
{
    internal class LobbyViewModel : BaseViewModel
    {
        private int selectedGameIndex;

        public LobbyViewModel()
        {
            var communicationService = new CommunicationService();
            OpenGames = new ObservableCollection<string>();
            JoinGame = new JoinGameCommand(this);
            NewGame = new NewGameCommand(communicationService);

            communicationService.NewOpenGameCallback = this.OnNewOpenGame;
            communicationService.Connect();
        }

        private void OnNewOpenGame(string gameId)
        {
            Application.Current.Dispatcher.Invoke(
                new Action(() => {
                    OpenGames.Add(gameId);
                }));
            
        }

        public ObservableCollection<string> OpenGames { get; set; }

        public JoinGameCommand JoinGame { get; set; }
        
        public ICommand NewGame { get; set; }

        public string? SelectedGameItem { get; set; }

        public int SelectedGameIndex        {
            get => selectedGameIndex; 
            set
            {
                selectedGameIndex = value;
                NotifyPropertyChanged();
            }
        }
    }
}
