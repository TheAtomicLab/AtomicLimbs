using Limbs.Web.Logic.Helpers;
using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Limbs.Web.Logic.Services
{
    [Authorize]
    public class MessagesHub : Hub
    {
        private readonly IMessageService _ms;
        private readonly ConnectionMapping<string> _connections;

        public MessagesHub(IMessageService ms, ConnectionMapping<string> connections)
        {
            _ms = ms;
            _connections = connections;
        }

        public void SendMessage(Guid threadId, string message)
        {
            Clients.OthersInGroup(threadId.ToString()).receiveMessage(message);
        }

        public override Task OnConnected()
        {
            var messageId = Guid.Parse(Context.QueryString["threadId"]);
            var message = _ms.View(Context.User, messageId);

            if (message == null)
            {
                throw new UnauthorizedAccessException();
            }

            string connectionId = Context.ConnectionId;
            string name = Context.User.Identity.Name;

            Groups.Add(connectionId, messageId.ToString());
            _connections.Add(name, connectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;
            _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;
            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
    }
}