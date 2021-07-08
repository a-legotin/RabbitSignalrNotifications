using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitSignalrNotifications.Shared
{
    public class RabbitMqSubscriber
    {
        private readonly string _queueName;
        private readonly string _exchangeName;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;
        
        private Func<byte[], Task<bool>> _messageHandler;
        private string _consumerTag = string.Empty;
        
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
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _channel.QueueBind(
                queue: queueName,
                exchange: _exchangeName,
                routingKey: routingKey);
            
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                if (_messageHandler == null
                    || (await _messageHandler(body)))
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            
            
        }

        public void SetMessageHandler(Func<byte[], Task<bool>> messageHandler)
            => _messageHandler = messageHandler;

        public void StartConsuming()
        {
            _consumerTag = _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: _consumer);
        }

        public void StopConsuming()
        {
            _channel.BasicCancel(_consumerTag);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}