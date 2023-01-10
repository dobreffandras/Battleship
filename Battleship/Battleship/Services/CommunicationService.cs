using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;
using Battleship.Model;
using static Battleship.Services.RabbitMqConstants;

namespace Battleship.Services
{
    internal class CommunicationService
    {
        private GameInfo? gameInfo;
        private readonly IConnection connection;
        private readonly IModel channel;

        public CommunicationService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" }; // TODO extract to config
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: ExchangeNames.OPEN_GAMES,
                type: ExchangeType.Fanout,
                durable: true);
        }

        public void Connect()
        {
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(
                queueName,
                exchange: ExchangeNames.OPEN_GAMES,
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
            if (message is not null)
            {
                NewOpenGameCallback?.Invoke(message);
            }
        }

        internal string StartNewGame()
        {
            var newGameId = Guid.NewGuid().ToString();

            SendNewGameMessage(newGameId);

            gameInfo = new GameInfo(newGameId, player: "a", otherPlayer: "b");

            channel.ExchangeDeclare(
                gameInfo.Exchange,
                ExchangeType.Direct,
                autoDelete: true,
                durable: true);

            DeclareAndBindQueueWithConsumer(
                gameInfo.ReceivingQueue,
                gameInfo.Exchange,
                gameInfo.ReceivingRoutingKeyIn,
                ShootMessageReceviced);

            DeclareAndBindQueueWithConsumer(
                gameInfo.ResponseQueue,
                gameInfo.Exchange,
                gameInfo.ResponseRoutingKeyIn,
                ShootResponseMessageReceviced);

            DeclareAndBindQueueWithConsumer(
                gameInfo.UtilityQueue,
                gameInfo.Exchange,
                gameInfo.UtilityRountingKeyIn,
                UtilityMessageReceviced);

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
                UtilityMessages.OPPONENT_CONNECTED => OpponentConnectedCallback,
                UtilityMessages.CONNECTION_ACCEPTED => ConnectionAcceptedCallback,
                UtilityMessages.OPPONENT_LEFT => OpponentLeftCallback,
                _ => null
            };

            action?.Invoke();
        }

        internal void JoinGame(string selectedGameItem)
        {
            SendGameDisapperedMessage(selectedGameItem);

            gameInfo = new GameInfo(selectedGameItem, player: "b", otherPlayer: "a");

            channel.ExchangeDeclare(
                gameInfo.Exchange,
                ExchangeType.Direct,
                autoDelete: true,
                durable: true);

            DeclareAndBindQueueWithConsumer(
                gameInfo.ReceivingQueue,
                gameInfo.Exchange,
                gameInfo.ReceivingRoutingKeyIn,
                ShootMessageReceviced);

            DeclareAndBindQueueWithConsumer(
                gameInfo.ResponseQueue,
                gameInfo.Exchange,
                gameInfo.ResponseRoutingKeyIn,
                ShootResponseMessageReceviced);

            DeclareAndBindQueueWithConsumer(
                gameInfo.UtilityQueue,
                gameInfo.Exchange,
                gameInfo.UtilityRountingKeyIn,
                UtilityMessageReceviced);

            SendConnectionMessage();
        }

        private void SendConnectionMessage()
        {
            channel.BasicPublish(
                exchange: gameInfo.Exchange,
                routingKey: gameInfo.UtilityRountingKeyOut,
                body: Encoding.UTF8.GetBytes(UtilityMessages.OPPONENT_CONNECTED));

        }

        internal void AcceptConnection()
        {
            channel.BasicPublish(
                exchange: gameInfo.Exchange,
                routingKey: gameInfo.UtilityRountingKeyOut,
                body: Encoding.UTF8.GetBytes(UtilityMessages.CONNECTION_ACCEPTED));
        }

        internal void LeaveGame()
        {
            channel.BasicPublish(
                exchange: gameInfo.Exchange,
                routingKey: gameInfo.UtilityRountingKeyOut,
                body: Encoding.UTF8.GetBytes(UtilityMessages.OPPONENT_LEFT));

            channel.QueueDelete(gameInfo.ReceivingQueue);
            channel.QueueDelete(gameInfo.ResponseQueue);
            channel.QueueDelete(gameInfo.UtilityQueue);
        }

        internal void Close()
        {
            connection.Close();
        }

        internal void Shoot((char x, char y) coord)
        {
            var gameMessageStr = JsonConvert.SerializeObject(
                new ShootMessage(coord.x, coord.y));

            channel.BasicPublish(
                exchange: gameInfo.Exchange,
                routingKey: gameInfo.ReceivingRoutingKeyOut,
                body: Encoding.UTF8.GetBytes(gameMessageStr));
        }

        internal void Respond((char x, char y) coord, bool isShippart, ShootState shootState)
        {
            var responseMessageStr = JsonConvert.SerializeObject(
                new ShootResponseMessage(coord.x, coord.y, isShippart, shootState));

            channel.BasicPublish(
                exchange: gameInfo.Exchange,
                routingKey: gameInfo.ResponseRoutingKeyOut,
                body: Encoding.UTF8.GetBytes(responseMessageStr));
        }

        private void SendNewGameMessage(string newGameId)
        {
            var message = new LobbyMessage(MessageType.NewGame, newGameId);

            channel.BasicPublish(
                exchange: ExchangeNames.OPEN_GAMES,
                routingKey: string.Empty,
                body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }

        private void SendGameDisapperedMessage(string selectedGameItem)
        {
            var message = new LobbyMessage(MessageType.GameDisappeared, selectedGameItem);

            channel.BasicPublish(
                exchange: ExchangeNames.OPEN_GAMES,
                routingKey: string.Empty,
                body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }

        private void DeclareAndBindQueueWithConsumer(
            string queue,
            string exchange,
            string routingKey,
            EventHandler<BasicDeliverEventArgs> eventHandler)
        {
            channel.QueueDeclare(queue, durable: true);
            channel.QueueBind(queue, exchange, routingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += eventHandler;
            channel.BasicConsume(
                queue: queue,
                consumer: consumer);
        }
    }
}
