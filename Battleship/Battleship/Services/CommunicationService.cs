using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Services
{
    internal class CommunicationService
    {
        private string newGameId;

        public CommunicationService()
        {
            newGameId = Guid.NewGuid().ToString();
        }

        public void Connect()
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

        public Action<string>? NewOpenGameCallback { get; set; }

        private void OpenGamesMessageReceived(object? sender, BasicDeliverEventArgs args)
        {
            var gameId = Encoding.UTF8.GetString(args.Body.ToArray());
            NewOpenGameCallback?.Invoke(gameId);
        }
    }
}
