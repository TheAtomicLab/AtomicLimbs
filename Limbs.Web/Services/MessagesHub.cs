using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Limbs.Web.Services
{
    [Authorize]
    public class MessagesHub : Hub
    {
        private readonly IMessageService _ms;

        public MessagesHub(IMessageService ms)
        {
            _ms = ms;
        }

        public void SendMessage(Guid threadId, string message)
        {
            Clients.OthersInGroup(threadId.ToString()).receiveMessage(message);
        }

        public override Task OnConnected()
        {
            var messageId = Guid.Parse(Context.QueryString["threadId"]);
            var message = _ms.View(Context.User, messageId);

            if (message == null) throw new UnauthorizedAccessException();

            Groups.Add(Context.ConnectionId, messageId.ToString());

            return base.OnConnected();
        }
    }
}