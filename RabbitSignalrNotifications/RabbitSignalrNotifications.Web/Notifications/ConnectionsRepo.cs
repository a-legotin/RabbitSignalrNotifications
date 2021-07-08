using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RabbitSignalrNotifications.Web.Notifications
{
    public static class ConnectionsRepo
    {
        public static IDictionary<Guid, List<string>> Clients = new ConcurrentDictionary<Guid, List<string>>();
    }
}