using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace RabbitSignalrNotifications.Web.Notifications
{
    public class NotificationHub : Hub
    {
        public NotificationHub()
        {
        }
        
        public override Task OnConnectedAsync()
        {
            var username = Guid.Parse(Context.GetHttpContext().Request.Query["user"]);

            if (ConnectionsRepo.Clients.TryGetValue(username, out var connections))
            {
                connections.Add(Context.ConnectionId);
            }
            else
            {
                var userConnections = new List<string>
                {
                    Context.ConnectionId
                };
                ConnectionsRepo.Clients[username] =  userConnections;
            }

            Task.Run(async () => await Groups.AddToGroupAsync(Context.ConnectionId, username.ToString(), CancellationToken.None));
            return base.OnConnectedAsync();
        }


        public override Task OnDisconnectedAsync(Exception ex)
        {
            foreach (var connections in ConnectionsRepo.Clients.Values)
            {
                if (connections.Contains(Context.ConnectionId))
                    connections.Remove(Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(ex);
        }
    }
}