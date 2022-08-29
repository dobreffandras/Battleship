using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship
{
    internal class LobbyViewModel
    {
        public LobbyViewModel()
        {
            var newGameId = Guid.NewGuid().ToString();
            OpenGames = new List<string> { newGameId };

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

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: "open_games",
                body: Encoding.UTF8.GetBytes(newGameId));
        }

        public IEnumerable<string> OpenGames { get; set; }
    }
}
