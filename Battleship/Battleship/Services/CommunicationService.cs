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
        private string? exchange;
        private string? player;
        private string? receivingQueue;
        private readonly IModel channel;

        public CommunicationService()
        {
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
            var newGameId = Guid.NewGuid().ToString();
            var messageStr = JsonConvert.SerializeObject(
                new LobbyMessage(MessageType.NewGame, newGameId));

            channel.BasicPublish(
                exchange: "open_games",
                routingKey: string.Empty,
                body: Encoding.UTF8.GetBytes(messageStr));

            receivingQueue = $"game-{newGameId}-receive-a";
            exchange = $"game-{newGameId}";
            player = "a";
            var otherPlayer = "b";

            channel.ExchangeDeclare(exchange, ExchangeType.Direct);
            channel.QueueDeclare(queue: receivingQueue);

            channel.QueueBind(
                receivingQueue,
                exchange,
                otherPlayer);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += GameActionMessageReceviced;
            channel.BasicConsume(
                queue: receivingQueue,
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


            receivingQueue = $"game-{selectedGameItem}-receive-b";
            exchange = $"game-{selectedGameItem}";
            player = "b";
            var otherPlayer = "a";

            channel.ExchangeDeclare(exchange, ExchangeType.Direct);
            channel.QueueDeclare(queue: receivingQueue);

            channel.QueueBind(
                receivingQueue,
                exchange,
                otherPlayer);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += GameActionMessageReceviced;
            channel.BasicConsume(
                queue: receivingQueue,
                consumer: consumer);
        }

        internal void Shoot((char x, char y) coord)
        {
            var gameMessageStr = JsonConvert.SerializeObject(
                new GameMessage(coord.x, coord.y));

            channel.BasicPublish(
                exchange: exchange,
                routingKey: player,
                body: Encoding.UTF8.GetBytes(gameMessageStr));
        }
    }
}
