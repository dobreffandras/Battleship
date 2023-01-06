using Battleship.Components;
using Battleship.Model;
using Battleship.Services;

namespace Battleship
{
    internal class GameViewModel : BaseViewModel
    {
        public GameViewModel(GameModel game, CommunicationService communicationService)
        {
            communicationService.ShootCallback = ShootMessageReceived;
            communicationService.ShootResponseCallback = ShootResponseMessageReceived;

            GameId = game.GameId;
            player = game.Player;
            GameState = game.State.Text;
            MyPlayingFieldViewModel = new PlayingFieldViewModel(game.MyPlayfieldModel, PlayingType.Passive, communicationService);
            OtherPlayingFieldViewModel = new PlayingFieldViewModel(game.OtherPlayfieldModel, PlayingType.Active, communicationService);
        }

        public string GameId { get; set; }

        private readonly Player player;

        public string PlayerSide => player == Player.PlayerOne ? "Player 1" : "Player 2";
        
        public PlayingFieldViewModel MyPlayingFieldViewModel { get; }

        public PlayingFieldViewModel OtherPlayingFieldViewModel { get; }

        public string GameState { get; set; }

        public void ShootMessageReceived(ShootMessage message)
        {
            MyPlayingFieldViewModel.ShootOn(message.X, message.Y);
            NotifyPropertyChanged(nameof(MyPlayingFieldViewModel));
        }
        
        public void ShootResponseMessageReceived(ShootResponseMessage message)
        {
            OtherPlayingFieldViewModel.SetCell(message.X, message.Y, message.IsShippart, message.ShootState);
            NotifyPropertyChanged(nameof(OtherPlayingFieldViewModel));
        }
    }
}