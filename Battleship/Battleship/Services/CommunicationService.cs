using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;

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
            var messageStr = JsonConvert.SerializeObject(new LobbyMessage(MessageType.NewGame, newGameId));

            channel.BasicPublish(
                exchange: "open_games",
                routingKey: string.Empty,
                body: Encoding.UTF8.GetBytes(messageStr));
        }
    }
}
