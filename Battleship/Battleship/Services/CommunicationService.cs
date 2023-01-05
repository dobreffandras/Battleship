using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Windows;
using Battleship.Components;

namespace Battleship.Services
{
    internal class CommunicationService
    {
        private string? exchange;
        private string? player;
        private string? receivingQueue;
        private string? responseQueue;
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
        
        public Action<GameResponseMessage>? GameResponseCallback { get; set; }

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
            responseQueue = $"game-{newGameId}-response-a";
            exchange = $"game-{newGameId}";
            player = "a";
            var otherPlayer = "b";

            channel.ExchangeDeclare(exchange, ExchangeType.Direct);
            channel.QueueDeclare(queue: receivingQueue);
            channel.QueueDeclare(queue: responseQueue);

            channel.QueueBind(
                receivingQueue,
                exchange,
                $"{otherPlayer}.receive");

            channel.QueueBind(
                responseQueue,
                exchange,
                $"{otherPlayer}.response");

            var consumer1 = new EventingBasicConsumer(channel);
            var consumer2 = new EventingBasicConsumer(channel);
            consumer1.Received += GameActionMessageReceviced;
            consumer2.Received += GameResponseMessageReceviced;
            channel.BasicConsume(
                queue: receivingQueue,
                consumer: consumer1);
            channel.BasicConsume(
                queue: responseQueue,
                consumer: consumer2);
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
        
        private void GameResponseMessageReceviced(object? sender, BasicDeliverEventArgs e)
        {
            var messageStr = Encoding.UTF8.GetString(e.Body.ToArray());
            var message = JsonConvert.DeserializeObject<GameResponseMessage>(messageStr);
            if (message is not null)
            {
                GameResponseCallback?.Invoke(message);
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
            responseQueue = $"game-{selectedGameItem}-response-b";
            exchange = $"game-{selectedGameItem}";
            player = "b";
            var otherPlayer = "a";

            channel.ExchangeDeclare(exchange, ExchangeType.Direct);
            channel.QueueDeclare(queue: receivingQueue);
            channel.QueueDeclare(queue: responseQueue);

            channel.QueueBind(
                receivingQueue,
                exchange,
                $"{otherPlayer}.receive");
            channel.QueueBind(
                responseQueue,
                exchange,
                $"{otherPlayer}.response");

            var consumer1 = new EventingBasicConsumer(channel);
            var consumer2 = new EventingBasicConsumer(channel);
            consumer1.Received += GameActionMessageReceviced;
            consumer2.Received += GameResponseMessageReceviced;
            channel.BasicConsume(
                queue: receivingQueue,
                consumer: consumer1);
            channel.BasicConsume(
                queue: responseQueue,
                consumer: consumer2);
        }

        internal void Shoot((char x, char y) coord)
        {
            var gameMessageStr = JsonConvert.SerializeObject(
                new GameMessage(coord.x, coord.y));

            channel.BasicPublish(
                exchange: exchange,
                routingKey: $"{player}.receive",
                body: Encoding.UTF8.GetBytes(gameMessageStr));
        }
        
        internal void Respond((char x, char y) coord, bool isShippart, ShootState shootState)
        {
            var responseMessageStr = JsonConvert.SerializeObject(
                new GameResponseMessage(coord.x, coord.y, isShippart, shootState));

            channel.BasicPublish(
                exchange: exchange,
                routingKey: $"{player}.response",
                body: Encoding.UTF8.GetBytes(responseMessageStr));
        }
    }
}
