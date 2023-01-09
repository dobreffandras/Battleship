using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;
using Battleship.Model;

namespace Battleship.Services
{
    internal class CommunicationService
    {
        private string? exchange;
        private string? player;
        private string? receivingQueue;
        private string? responseQueue;
        private string? utilityQueue;
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

        public Action<ShootMessage>? ShootCallback { get; set; }
        
        public Action<ShootResponseMessage>? ShootResponseCallback { get; set; }
        
        public Action? OpponentConnectedCallback { get; set; }

        public Action? ConnectionAcceptedCallback { get; set; }
        
        public Action? OpponentLeftCallback { get; set; }

        private void OpenGamesMessageReceived(object? sender, BasicDeliverEventArgs args)
        {
            var messageStr = Encoding.UTF8.GetString(args.Body.ToArray());
            var message = JsonConvert.DeserializeObject<LobbyMessage>(messageStr);
            if(message is not null)
            {
                NewOpenGameCallback?.Invoke(message);
            }
        }

        internal string StartNewGame()
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
            utilityQueue = $"game-{newGameId}-utility-a";
            exchange = $"game-{newGameId}";
            player = "a";
            var otherPlayer = "b";

            channel.ExchangeDeclare(exchange, ExchangeType.Direct);
            channel.QueueDeclare(queue: receivingQueue);
            channel.QueueDeclare(queue: responseQueue);
            channel.QueueDeclare(queue: utilityQueue);

            channel.QueueBind(
                receivingQueue,
                exchange,
                $"{otherPlayer}.receive");
            channel.QueueBind(
                responseQueue,
                exchange,
                $"{otherPlayer}.response");
            channel.QueueBind(
                utilityQueue,
                exchange,
                $"{otherPlayer}.utility");

            var consumer1 = new EventingBasicConsumer(channel);
            var consumer2 = new EventingBasicConsumer(channel);
            var consumer3 = new EventingBasicConsumer(channel);
            consumer1.Received += ShootMessageReceviced;
            consumer2.Received += ShootResponseMessageReceviced;
            consumer3.Received += UtilityMessageReceviced;
            channel.BasicConsume(
                queue: receivingQueue,
                consumer: consumer1);
            channel.BasicConsume(
                queue: responseQueue,
                consumer: consumer2);
            channel.BasicConsume(
                queue: utilityQueue,
                consumer: consumer3);

            return newGameId;
        }

        private void ShootMessageReceviced(object? sender, BasicDeliverEventArgs e)
        {
            var messageStr = Encoding.UTF8.GetString(e.Body.ToArray());
            var message = JsonConvert.DeserializeObject<ShootMessage>(messageStr);
            if (message is not null)
            {
                ShootCallback?.Invoke(message);
            }
        }
        
        private void ShootResponseMessageReceviced(object? sender, BasicDeliverEventArgs e)
        {
            var messageStr = Encoding.UTF8.GetString(e.Body.ToArray());
            var message = JsonConvert.DeserializeObject<ShootResponseMessage>(messageStr);
            if (message is not null)
            {
                ShootResponseCallback?.Invoke(message);
            }
        }
        
        private void UtilityMessageReceviced(object? sender, BasicDeliverEventArgs e)
        {
            var messageStr = Encoding.UTF8.GetString(e.Body.ToArray());
            var action = messageStr switch
            {
                "opponentConnected" => OpponentConnectedCallback,
                "connectionAccepted" => ConnectionAcceptedCallback,
                "opponentLeft" => OpponentLeftCallback,
                _ => null
            };

            action?.Invoke();
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
            utilityQueue = $"game-{selectedGameItem}-utility-b";
            exchange = $"game-{selectedGameItem}";
            player = "b";
            var otherPlayer = "a";

            channel.ExchangeDeclare(exchange, ExchangeType.Direct);
            channel.QueueDeclare(queue: receivingQueue);
            channel.QueueDeclare(queue: responseQueue);
            channel.QueueDeclare(queue: utilityQueue);

            channel.QueueBind(
                receivingQueue,
                exchange,
                $"{otherPlayer}.receive");
            channel.QueueBind(
                responseQueue,
                exchange,
                $"{otherPlayer}.response");
            channel.QueueBind(
                utilityQueue,
                exchange,
                $"{otherPlayer}.utility");

            var consumer1 = new EventingBasicConsumer(channel);
            var consumer2 = new EventingBasicConsumer(channel);
            var consumer3 = new EventingBasicConsumer(channel);
            consumer1.Received += ShootMessageReceviced;
            consumer2.Received += ShootResponseMessageReceviced;
            consumer3.Received += UtilityMessageReceviced;
            channel.BasicConsume(
                queue: receivingQueue,
                consumer: consumer1);
            channel.BasicConsume(
                queue: responseQueue,
                consumer: consumer2);
            channel.BasicConsume(
                queue: utilityQueue,
                consumer: consumer3);

            SendConnectionMessage();
        }

        private void SendConnectionMessage()
        {
            channel.BasicPublish(
                exchange: exchange,
                routingKey: $"{player}.utility",
                body: Encoding.UTF8.GetBytes("opponentConnected"));
        }

        internal void AcceptConnection()
        {
            channel.BasicPublish(
                exchange: exchange,
                routingKey: $"{player}.utility",
                body: Encoding.UTF8.GetBytes("connectionAccepted"));
        }

        internal void LeaveGame()
        {
            channel.BasicPublish(
                exchange: exchange,
                routingKey: $"{player}.utility",
                body: Encoding.UTF8.GetBytes("opponentLeft"));
        }

        internal void Shoot((char x, char y) coord)
        {
            var gameMessageStr = JsonConvert.SerializeObject(
                new ShootMessage(coord.x, coord.y));

            channel.BasicPublish(
                exchange: exchange,
                routingKey: $"{player}.receive",
                body: Encoding.UTF8.GetBytes(gameMessageStr));
        }
        
        internal void Respond((char x, char y) coord, bool isShippart, ShootState shootState)
        {
            var responseMessageStr = JsonConvert.SerializeObject(
                new ShootResponseMessage(coord.x, coord.y, isShippart, shootState));

            channel.BasicPublish(
                exchange: exchange,
                routingKey: $"{player}.response",
                body: Encoding.UTF8.GetBytes(responseMessageStr));
        }
    }
}
