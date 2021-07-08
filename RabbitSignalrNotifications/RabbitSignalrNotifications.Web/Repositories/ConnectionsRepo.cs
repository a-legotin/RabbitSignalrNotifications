using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RabbitSignalrNotifications.Web.Repositories
{
    internal class ConnectionsRepo : IConnectionsRepo
    {
        public IDictionary<Guid, List<string>> Clients { get; } = new ConcurrentDictionary<Guid, List<string>>();
    }
}