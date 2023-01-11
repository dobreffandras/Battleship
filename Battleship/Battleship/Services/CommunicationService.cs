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
        private readonly IConnection connection;
        private readonly IModel channel;

        public CommunicationService(string hostName)
        {
            var factory = new ConnectionFactory() { HostName = hostName };
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

        internal GameMetadata StartNewGame()
        {
            var newGameId = Guid.NewGuid().ToString();

            SendNewGameMessage(newGameId);

            var gameMeta =
                new GameMetadata(
                    newGameId, 
                    player: Player.PlayerOne, 
                    otherPlayer: Player.PlayerTwo);

            channel.ExchangeDeclare(
                gameMeta.Exchange,
                ExchangeType.Direct,
                autoDelete: true,
                durable: true);

            DeclareAndBindQueueWithConsumer(
                gameMeta.ReceivingQueue,
                gameMeta.Exchange,
                gameMeta.ReceivingRoutingKeyIn,
                ShootMessageReceviced);

            DeclareAndBindQueueWithConsumer(
                gameMeta.ResponseQueue,
                gameMeta.Exchange,
                gameMeta.ResponseRoutingKeyIn,
                ShootResponseMessageReceviced);

            DeclareAndBindQueueWithConsumer(
                gameMeta.UtilityQueue,
                gameMeta.Exchange,
                gameMeta.UtilityRountingKeyIn,
                UtilityMessageReceviced);

            return gameMeta;
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

        internal GameMetadata JoinGame(string selectedGameItem)
        {
            SendGameDisapperedMessage(selectedGameItem);

            var gameMeta = 
                new GameMetadata(
                    selectedGameItem, 
                    player: Player.PlayerTwo, 
                    otherPlayer: Player.PlayerOne);

            channel.ExchangeDeclare(
                gameMeta.Exchange,
                ExchangeType.Direct,
                autoDelete: true,
                durable: true);

            DeclareAndBindQueueWithConsumer(
                gameMeta.ReceivingQueue,
                gameMeta.Exchange,
                gameMeta.ReceivingRoutingKeyIn,
                ShootMessageReceviced);

            DeclareAndBindQueueWithConsumer(
                gameMeta.ResponseQueue,
                gameMeta.Exchange,
                gameMeta.ResponseRoutingKeyIn,
                ShootResponseMessageReceviced);

            DeclareAndBindQueueWithConsumer(
                gameMeta.UtilityQueue,
                gameMeta.Exchange,
                gameMeta.UtilityRountingKeyIn,
                UtilityMessageReceviced);

            SendConnectionMessage(gameMeta);

            return gameMeta;
        }

        private void SendConnectionMessage(GameMetadata gameMeta)
        {
            channel.BasicPublish(
                exchange: gameMeta.Exchange,
                routingKey: gameMeta.UtilityRountingKeyOut,
                body: Encoding.UTF8.GetBytes(UtilityMessages.OPPONENT_CONNECTED));
        }

        internal void AcceptConnection(GameMetadata gameMeta)
        {
            channel.BasicPublish(
                exchange: gameMeta.Exchange,
                routingKey: gameMeta.UtilityRountingKeyOut,
                body: Encoding.UTF8.GetBytes(UtilityMessages.CONNECTION_ACCEPTED));
        }

        internal void LeaveGame(GameMetadata gameMeta)
        {
            SendOpponentLeftMessage(gameMeta);

            if (gameMeta.Player == Player.PlayerOne)
            {
                SendGameDisapperedMessage(gameMeta.GameId);
            }

            channel.QueueDelete(gameMeta.ReceivingQueue);
            channel.QueueDelete(gameMeta.ResponseQueue);
            channel.QueueDelete(gameMeta.UtilityQueue);
        }

        internal void Close()
        {
            connection.Close();
        }

        internal void Shoot(GameMetadata gameMeta, (char x, char y) coord)
        {
            var gameMessageStr = JsonConvert.SerializeObject(
                new ShootMessage(coord.x, coord.y));

            channel.BasicPublish(
                exchange: gameMeta.Exchange,
                routingKey: gameMeta.ReceivingRoutingKeyOut,
                body: Encoding.UTF8.GetBytes(gameMessageStr));
        }

        internal void Respond(GameMetadata gameMeta, (char x, char y) coord, bool isShippart, ShootState shootState)
        {
            var responseMessageStr = JsonConvert.SerializeObject(
                new ShootResponseMessage(coord.x, coord.y, isShippart, shootState));

            channel.BasicPublish(
                exchange: gameMeta.Exchange,
                routingKey: gameMeta.ResponseRoutingKeyOut,
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

        private void SendGameDisapperedMessage(string gameId)
        {
            var message = new LobbyMessage(MessageType.GameDisappeared, gameId);

            channel.BasicPublish(
                exchange: ExchangeNames.OPEN_GAMES,
                routingKey: string.Empty,
                body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }

        private void SendOpponentLeftMessage(GameMetadata gameMeta)
        {
            channel.BasicPublish(
                exchange: gameMeta.Exchange,
                routingKey: gameMeta.UtilityRountingKeyOut,
                body: Encoding.UTF8.GetBytes(UtilityMessages.OPPONENT_LEFT));
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
