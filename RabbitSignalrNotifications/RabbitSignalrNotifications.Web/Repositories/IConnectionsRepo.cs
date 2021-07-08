using System;
using System.Collections.Generic;

namespace RabbitSignalrNotifications.Web.Repositories
{
    internal interface IConnectionsRepo
    {
        IDictionary<Guid, List<string>> Clients { get; }
    }
}