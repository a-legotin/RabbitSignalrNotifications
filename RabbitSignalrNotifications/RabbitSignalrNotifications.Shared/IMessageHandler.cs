using System.Threading.Tasks;

namespace RabbitSignalrNotifications.Shared
{
    public interface IMessageHandler<T>
    {
        Task<bool> Handle(T obj);
    }
}