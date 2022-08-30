using Battleship.Commands;
using Battleship.Components;
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
            Action<PlayfieldModel> navigateToGameViewModel)
        {
            NewGamePlayField = new PreparingPlayfieldViewModel(new PlayfieldModel());
            OpenGames = new ObservableCollection<string>();
            JoinGame = new JoinGameCommand(this, navigateToGameViewModel);
            NewGame = new NewGameCommand(this, communicationService, navigateToGameViewModel);
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

        internal void JoinGameAction()
        {
            if(SelectedGameItem is not null)
            {
                communicationService.JoinGame(SelectedGameItem);
            }
        }

        public ObservableCollection<string> OpenGames { get; set; }

        public JoinGameCommand JoinGame { get; set; }

        public PreparingPlayfieldViewModel NewGamePlayField { get; }

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
