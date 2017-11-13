using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Limbs.Web.Entities.Models;
using Limbs.Web.Extensions;
using Limbs.Web.Models;
using Microsoft.AspNet.Identity;

namespace Limbs.Web.Services
{
    public class MessagesService : IMessageService
    {
        public ApplicationDbContext Db;

        public MessagesService()
        {
            Db = new ApplicationDbContext();
        }
        public MessagesService(ApplicationDbContext db)
        {
            Db = db;
        }

        public async Task<int> Send(IPrincipal user, MessageModel message)
        {
            if (message.PreviousMessage != null) //add to thread
            {
                message.Status = MessageStatus.Unread;
                message.Priority = message.PreviousMessage.Priority;
                message.PreviousMessage.Status = MessageStatus.Unread;
                if (message.From.Equals(message.PreviousMessage.From)) //adding to your message
                {
                    message.To = message.PreviousMessage.To;
                }
                else
                {
                    message.To = message.PreviousMessage.From;
                }
            }
            Db.Messages.Add(message);

            return await Db.SaveChangesAsync();
        }

        public async Task<IEnumerable<MessageModel>> GetAllMessages(IPrincipal user)
        {
            if(user.IsInRole(AppRoles.Administrator))
                return await Db.Messages.Include(x => x.From).Include(x => x.To).OrderByDescending(x => x.Time).ToListAsync();
            return await GetInboxMessages(user);
        }

        public async Task<IEnumerable<MessageModel>> GetInboxMessages(IPrincipal user)
        {
            var userId = user.Identity.GetUserId();

            return await Db.Messages.Include(x => x.From).Include(x => x.To)
                .Where(x => (x.To.Id == userId || x.From.Id == userId) && x.Status != MessageStatus.Deleted && x.PreviousMessage == null)
                .OrderByDescending(x => x.Time)
                .ToListAsync();
        }

        public async Task<IEnumerable<MessageModel>> GetInboxMessages(IPrincipal user, int? orderId)
        {
            if (!orderId.HasValue) return await GetInboxMessages(user);

            var userId = user.Identity.GetUserId();

            return await Db.Messages.Include(x => x.From).Include(x => x.To)
                .Where(x => (x.To.Id == userId || x.From.Id == userId) && x.Order.Id == orderId.Value && x.Status != MessageStatus.Deleted && x.PreviousMessage == null)
                .OrderByDescending(x => x.Time)
                .ToListAsync();
        }

        public async Task<IEnumerable<MessageModel>> GetThreadMessages(IPrincipal user, Guid mainMessageId)
        {
            var mainMessage = await View(user, mainMessageId);
            return await GetThreadMessages(user, mainMessage);
        }

        public async Task<IEnumerable<MessageModel>> GetThreadMessages(IPrincipal user, MessageModel mainMessage)
        {
            var userId = user.Identity.GetUserId();

            if (mainMessage.To.Id != userId && mainMessage.From.Id != userId) return null;

            return await Db.Messages.Include(x => x.From).Include(x => x.To)
                .Where(x => x.Status != MessageStatus.Deleted && x.PreviousMessage.Id == mainMessage.Id)
                .OrderByDescending(x => x.Time)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCount(IPrincipal user)
        {
            var userId = user.Identity.GetUserId();

            return await Db.Messages.Include(x => x.From)
                .Where(x => x.To.Id == userId && x.Status == MessageStatus.Unread).CountAsync();
        }

        public async Task<int> GetUnreadCount(IPrincipal user, MessageModel mainMessage)
        {
            if (mainMessage == null) return -1;

            var userId = user.Identity.GetUserId();

            return await Db.Messages.Include(x => x.From)
                .Where(x => x.To.Id == userId && 
                            x.PreviousMessage.Id == mainMessage.Id && 
                            x.Status == MessageStatus.Unread).CountAsync();
        }

        public async Task<int> MarkAsRead(IPrincipal user, MessageModel message)
        {
            if (message.Status != MessageStatus.Unread || !user.IsDestination(message)) return 0;
            
            message.Status = MessageStatus.Read;
            if (await GetUnreadCount(user, message.PreviousMessage) == 0)
            {
                message.PreviousMessage.Status = MessageStatus.Read;
            }

            return await Db.SaveChangesAsync();
        }

        public async Task<int> Delete(IPrincipal user, Guid id)
        {
            var messageModel = await View(user, id);

            if (messageModel == null) return 0;

            messageModel.Status = MessageStatus.Deleted;

            return await Db.SaveChangesAsync();
        }

        public async Task<MessageModel> View(IPrincipal user, Guid id)
        {
            var message = await Db.Messages.Include(x => x.To).Include(x => x.From)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (user.IsInMessage(message))
            {
                await MarkAsRead(user, message);

                return message;
            }

            if (user.IsInRole(AppRoles.Administrator))
                return message;

            return null;
        }
    }
}