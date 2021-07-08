using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace RabbitSignalrNotifications.Shared
{
    public class RabbitMqPublisher
    {
        private readonly IModel _channel;

        private readonly IConnection _connection;
        private readonly string _exchangeName;
        private readonly IBasicProperties _properties;

        public RabbitMqPublisher(string exchangeName, IConnectionFactory connectionFactory)
        {
            _exchangeName = exchangeName;

            _connection = connectionFactory.CreateConnection();

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(
                    _exchangeName,
                    ExchangeType.Direct,
                    true);
            }

            _channel = _connection.CreateModel();
            _properties = _channel.CreateBasicProperties();
            _properties.Persistent = true;
        }


        public void Publish<T>(string routingKey, T data)
        {
            _channel.BasicPublish(
                _exchangeName,
                routingKey,
                _properties,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data)));
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}