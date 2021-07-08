using RabbitMQ.Client;

namespace RabbitSignalrNotifications.Shared
{
    public class RabbitMqPublisher
    {
        private readonly string _exchangeName;

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IBasicProperties _properties;

        public RabbitMqPublisher(string exchangeName, IConnectionFactory connectionFactory)
        {
            _exchangeName = exchangeName;

            _connection = connectionFactory.CreateConnection();

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(
                    exchange: _exchangeName,
                    type: ExchangeType.Direct,
                    durable: true);
            }

            _channel = _connection.CreateModel();
            _properties = _channel.CreateBasicProperties();
            _properties.Persistent = true;
        }


        public void Publish(string routingKey, byte[] data)
        {
            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: _properties,
                body: data);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}