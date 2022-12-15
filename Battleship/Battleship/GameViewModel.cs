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

            MyPlayingFieldViewModel = new PlayingFieldViewModel(myModel, communicationService);
            OtherPlayingFieldViewModel = new PlayingFieldViewModel(otherModel, communicationService);
        }

        public PlayingFieldViewModel MyPlayingFieldViewModel { get; }
        public PlayingFieldViewModel OtherPlayingFieldViewModel { get; }

        public void ChangeMessageReceived(GameMessage message)
        {
            MyPlayingFieldViewModel.ShootOn(message.X, message.Y);
            NotifyPropertyChanged(nameof(MyPlayingFieldViewModel));
        }
    }
}