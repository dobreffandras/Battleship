using Battleship.Commands;
using Battleship.Components;
using Battleship.Model;
using Battleship.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Battleship
{
    internal class LobbyViewModel : BaseViewModel
    {
        private int selectedGameIndex;
        private CommunicationService communicationService;

        public LobbyViewModel(
            CommunicationService communicationService,
            NavigationService navigationService)
        {
            NewGamePlayField = new PreparingPlayfieldViewModel(new PlayfieldModel());
            OpenGames = new ObservableCollection<string>();
            JoinGameCommand = new JoinGameCommand(this, navigationService);
            NewGameCommand = new NewGameCommand(this, navigationService);
            this.communicationService = communicationService;
            
            communicationService.NewOpenGameCallback = this.OnNewOpenGame;
            communicationService.Connect();
        }

        private void OnNewOpenGame(LobbyMessage message)
        {
            var gameId = message.GameGuid;
            switch (message.Type)
            {
                case MessageType.NewGame:
                    Application.Current.Dispatcher.Invoke(
                        new Action(() => {
                            if (!OpenGames.Contains(gameId))
                            {
                                OpenGames.Add(gameId);
                            }
                        }));
                    break;
                case MessageType.GameDisappeared:

                    Application.Current.Dispatcher.Invoke(
                        new Action(() => {
                            OpenGames.Remove(gameId);
                        }));
                    break;
            }
        }

        internal GameModel CreateNewGame()
        {
            var gameMeta = communicationService.StartNewGame();
            return new GameModel(gameMeta, NewGamePlayField.Model);
        }

        internal GameModel JoinGame()
        {
            // Should not be called without selected game
            if (SelectedGameItem is null) throw new ArgumentNullException(nameof(SelectedGameItem));

            var gameMeta = communicationService.JoinGame(SelectedGameItem);
            return new GameModel(gameMeta, NewGamePlayField.Model);
        }

        public ObservableCollection<string> OpenGames { get; set; }

        public ICommand JoinGameCommand { get; set; }

        public PreparingPlayfieldViewModel NewGamePlayField { get; }

        public ICommand NewGameCommand { get; set; }

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
