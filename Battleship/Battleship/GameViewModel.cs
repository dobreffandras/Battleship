using Battleship.Components;
using Battleship.Model;
using Battleship.Services;

namespace Battleship
{
    internal class GameViewModel : BaseViewModel
    {
        public GameViewModel(GameModel game, PlayfieldModel otherModel, CommunicationService communicationService)
        {
            communicationService.GameActionCallback = ChangeMessageReceived;
            communicationService.GameResponseCallback = ResponseMessageReceived;

            GameId = game.GameId;
            player = game.Player;
            MyPlayingFieldViewModel = new PlayingFieldViewModel(game.PlayfieldModel, PlayingType.Passive, communicationService);
            OtherPlayingFieldViewModel = new PlayingFieldViewModel(otherModel, PlayingType.Active, communicationService);
        }

        public string GameId { get; set; }

        private readonly Player player;

        public string PlayerSide => player == Player.PlayerOne ? "Player 1" : "Player 2";
        
        public PlayingFieldViewModel MyPlayingFieldViewModel { get; }

        public PlayingFieldViewModel OtherPlayingFieldViewModel { get; }

        public void ChangeMessageReceived(GameMessage message)
        {
            MyPlayingFieldViewModel.ShootOn(message.X, message.Y);
            NotifyPropertyChanged(nameof(MyPlayingFieldViewModel));
        }
        
        public void ResponseMessageReceived(GameResponseMessage message)
        {
            OtherPlayingFieldViewModel.SetCell(message.X, message.Y, message.IsShippart, message.ShootState);
            NotifyPropertyChanged(nameof(OtherPlayingFieldViewModel));
        }
    }
}