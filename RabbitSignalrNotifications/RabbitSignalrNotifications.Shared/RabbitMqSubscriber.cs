using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitSignalrNotifications.Shared
{
    public class RabbitMqSubscriber<T> : IDisposable
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly EventingBasicConsumer _consumer;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private string _consumerTag = string.Empty;

        private Func<T, Task<bool>> _messageHandler;

        public RabbitMqSubscriber(
            string queueName,
            string exchangeName,
            string routingKey,
            IConnectionFactory connectionFactory)
        {
            _queueName = queueName;
            _exchangeName = exchangeName;

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                _queueName,
                true,
                false,
                false,
                null);
            _channel.QueueBind(
                queueName,
                _exchangeName,
                routingKey);

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
                if (_messageHandler == null
                    || await _messageHandler(message))
                    _channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }

        public void SetMessageHandler(Func<T, Task<bool>> messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public void StartConsuming()
        {
            _consumerTag = _channel.BasicConsume(
                _queueName,
                false,
                _consumer);
        }

        public void StopConsuming()
        {
            _channel.BasicCancel(_consumerTag);
        }
    }
}