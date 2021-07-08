using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitSignalrNotifications.Shared
{
    public static class RabbitConnectionFactory
    {
        public static IConnectionFactory GetDefault()
        {
            return new ConnectionFactory
            {
                Uri = RabbitMqOptions.RabbitUrl
            };
        }
    }

    public interface IMessageHandler<T>
    {
        Task<bool> Handle(T obj);
    }
}