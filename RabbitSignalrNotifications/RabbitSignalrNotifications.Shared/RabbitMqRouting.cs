namespace RabbitSignalrNotifications.Shared
{
    public static class RabbitMqRouting
    {
        public static string WebToService { get; } = "web-service";
        public static string ServiceToWeb { get; } = "service-web";
    }
}