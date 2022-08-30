using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Windows;

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

            channel.ExchangeDeclare(
                exchange: "open_games",
                type: ExchangeType.Fanout);
        }

        public void Connect()
        {
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(
                queueName,
                exchange: "open_games",
                routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OpenGamesMessageReceived;
            channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);
        }

        public Action<LobbyMessage>? NewOpenGameCallback { get; set; }

        public Action<GameMessage>? GameActionCallback { get; set; }

        private void OpenGamesMessageReceived(object? sender, BasicDeliverEventArgs args)
        {
            var messageStr = Encoding.UTF8.GetString(args.Body.ToArray());
            var message = JsonConvert.DeserializeObject<LobbyMessage>(messageStr);
            if(message is not null)
            {
                NewOpenGameCallback?.Invoke(message);
            }
        }

        internal void StartNewGame()
        {
            var messageStr = JsonConvert.SerializeObject(
                new LobbyMessage(MessageType.NewGame, newGameId));

            channel.BasicPublish(
                exchange: "open_games",
                routingKey: string.Empty,
                body: Encoding.UTF8.GetBytes(messageStr));

            var gameQueue = $"game-{newGameId}";
            channel.QueueDeclare(queue: gameQueue);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += GameActionMessageReceviced;
            channel.BasicConsume(
                queue: gameQueue,
                consumer: consumer);
        }

        private void GameActionMessageReceviced(object? sender, BasicDeliverEventArgs e)
        {
            var messageStr = Encoding.UTF8.GetString(e.Body.ToArray());
            var message = JsonConvert.DeserializeObject<GameMessage>(messageStr);
            if (message is not null)
            {
                GameActionCallback?.Invoke(message);
            }
        }

        internal void JoinGame(string selectedGameItem)
        {
            var messageStr = JsonConvert.SerializeObject(
                new LobbyMessage(MessageType.GameDisappeared, selectedGameItem));

            channel.BasicPublish(
                exchange: "open_games",
                routingKey: string.Empty,
                body: Encoding.UTF8.GetBytes(messageStr));

            // TODO this is a dummy message
            var gameQueue = $"game-{selectedGameItem}";
            var gameMessageStr = JsonConvert.SerializeObject(
                new GameMessage('B', '2')); 

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: gameQueue,
                body: Encoding.UTF8.GetBytes(gameMessageStr));
        }
    }
}
