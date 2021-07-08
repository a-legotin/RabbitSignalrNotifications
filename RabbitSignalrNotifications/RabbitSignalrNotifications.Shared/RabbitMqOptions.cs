using System;

namespace RabbitSignalrNotifications.Shared
{
    public static class RabbitMqOptions
    {
        public static Uri RabbitUrl { get; } = new("amqp://admin:admin@docker.server.codegarage.local:5672/");
        public static string RabbitExchange { get; } = "RabbitSignalrNotifications";
    }
}