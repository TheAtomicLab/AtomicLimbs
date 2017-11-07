using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Services
{
    public interface IMessageService
    {
        Task<int> Send(IPrincipal user, MessageModel message);

        Task<IEnumerable<MessageModel>> GetAllMessages(IPrincipal user);

        Task<IEnumerable<MessageModel>> GetInboxMessages(IPrincipal user);

        Task<int> GetUnreadCount(IPrincipal user);

        Task<int> MarkAsRead(IPrincipal user, MessageModel message);

        Task<int> Delete(IPrincipal user, Guid id);

        Task<MessageModel> View(IPrincipal user, Guid id);
    }
}
