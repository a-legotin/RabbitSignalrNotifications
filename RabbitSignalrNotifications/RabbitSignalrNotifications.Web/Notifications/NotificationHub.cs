using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using RabbitSignalrNotifications.Web.Repositories;

namespace RabbitSignalrNotifications.Web.Notifications
{
    internal class NotificationHub : Hub
    {
        private readonly IConnectionsRepo _connectionsRepo;

        public NotificationHub(IConnectionsRepo connectionsRepo)
        {
            _connectionsRepo = connectionsRepo;
        }
        
        public override Task OnConnectedAsync()
        {
            var username = Guid.Parse(Context.GetHttpContext().Request.Query["user"]);

            if (_connectionsRepo.Clients.TryGetValue(username, out var connections))
            {
                connections.Add(Context.ConnectionId);
            }
            else
            {
                var userConnections = new List<string>
                {
                    Context.ConnectionId
                };
                _connectionsRepo.Clients[username] =  userConnections;
            }

            Task.Run(async () => await Groups.AddToGroupAsync(Context.ConnectionId, username.ToString(), CancellationToken.None));
            return base.OnConnectedAsync();
        }


        public override Task OnDisconnectedAsync(Exception ex)
        {
            foreach (var connections in _connectionsRepo.Clients.Values)
            {
                if (connections.Contains(Context.ConnectionId))
                    connections.Remove(Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(ex);
        }
    }
}