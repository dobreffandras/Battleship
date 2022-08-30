using Battleship.Services;

namespace Battleship
{
    internal class GameViewModel : BaseViewModel
    {
        private readonly CommunicationService communicationService;
        private string? messageReceived;

        public GameViewModel(CommunicationService communicationService)
        {
            this.communicationService = communicationService;
            communicationService.GameActionCallback = ChangeMessageReceived;
        }

        public string? MessageReceived
        {
            get => messageReceived;
            set
            {
                messageReceived = value;
                NotifyPropertyChanged();
            }
        }

        public void ChangeMessageReceived(string messageReceived)
        {
            this.MessageReceived = messageReceived;
        }
    }
}