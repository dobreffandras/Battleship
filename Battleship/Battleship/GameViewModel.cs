using Battleship.Components;
using Battleship.Services;

namespace Battleship
{
    internal class GameViewModel : BaseViewModel
    {
        private readonly CommunicationService communicationService;

        public GameViewModel(PlayfieldModel myModel, PlayfieldModel otherModel, CommunicationService communicationService)
        {
            this.communicationService = communicationService;
            communicationService.GameActionCallback = ChangeMessageReceived;
            communicationService.GameResponseCallback = ResponseMessageReceived;

            MyPlayingFieldViewModel = new PlayingFieldViewModel(myModel, PlayingType.Passive, communicationService);
            OtherPlayingFieldViewModel = new PlayingFieldViewModel(otherModel, PlayingType.Active, communicationService);
        }

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