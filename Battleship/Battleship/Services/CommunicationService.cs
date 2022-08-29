using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace Battleship.Services
{
    internal class CommunicationService
    {
        private readonly string newGameId;
        private readonly IModel channel;

        public CommunicationService()
        {
            newGameId = Guid.NewGuid().ToString();
            var factory = new ConnectionFactory() { HostName = "localhost" }; // TODO extract to config
            var connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "open_games", // TODO extract magic constant
                durable: false,
                exclusive: false,
                autoDelete: false);
        }

        public void Connect()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OpenGamesMessageReceived;
            channel.BasicConsume(
                queue: "open_games",
                autoAck: false,
                consumer: consumer);
        }

        public Action<string>? NewOpenGameCallback { get; set; }

        private void OpenGamesMessageReceived(object? sender, BasicDeliverEventArgs args)
        {
            var gameId = Encoding.UTF8.GetString(args.Body.ToArray());
            NewOpenGameCallback?.Invoke(gameId);
        }

        internal void StartNewGame()
        {
            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: "open_games",
                body: Encoding.UTF8.GetBytes(newGameId));
        }
    }
}
