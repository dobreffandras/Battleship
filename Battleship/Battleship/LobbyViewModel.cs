using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Battleship
{
    internal class LobbyViewModel : BaseViewModel
    {
        private int selectedGameIndex;

        public LobbyViewModel()
        {
            var newGameId = Guid.NewGuid().ToString();
            OpenGames = new ObservableCollection<string>();
            JoinGame = new JoinGameCommand(this);
            SendOpenGameMessage(newGameId);
        }

        private void SendOpenGameMessage(string newGameId)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" }; // TODO extract to config
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "open_games", // TODO extract magic constant
                durable: false,
                exclusive: false,
                autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OpenGamesMessageReceived;
            channel.BasicConsume(
                queue: "open_games",
                autoAck: false,
                consumer: consumer);

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: "open_games",
                body: Encoding.UTF8.GetBytes(newGameId));
        }

        private void OpenGamesMessageReceived(object? sender, BasicDeliverEventArgs args)
        {
            var gameId = Encoding.UTF8.GetString(args.Body.ToArray());
            OpenGames.Add(gameId);
        }

        public ObservableCollection<string> OpenGames { get; set; }

        public JoinGameCommand JoinGame { get; set; }

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
