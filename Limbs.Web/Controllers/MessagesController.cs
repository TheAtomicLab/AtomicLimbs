using System;
using System.Configuration;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Common.Mail;
using Limbs.Web.Common.Mail.Entities;
using Limbs.Web.Entities.Models;
using Limbs.Web.Helpers;
using Limbs.Web.Services;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Microsoft.AspNet.Identity;

namespace Limbs.Web.Controllers
{
    [DefaultAuthorize(Roles = AppRoles.User + "," + AppRoles.Administrator)]
    public class MessagesController : BaseController
    {
        private readonly string _fromEmail = ConfigurationManager.AppSettings["Mail.From"];

        private readonly IMessageService _ms;
        private readonly ConnectionMapping<string> _connections;

        public MessagesController(IMessageService ms, ConnectionMapping<string> connections)
        {
            _ms = new MessagesService(Db);
            _connections = connections;
        }

        // GET: Messages
        public ActionResult Index()
        {
            return RedirectToAction("Inbox");
        }

        // GET: Messages/Inbox
        public ActionResult Inbox()
        {
            return View();
        }

        // GET: Messages/InboxPartial
        public async Task<ActionResult> InboxPartial(int? orderId)
        {
            var messages = await _ms.GetInboxMessages(User, orderId);

            return PartialView("_InboxPartial", messages);
        }

        // GET: Messages/ThreadMessages
        public async Task<ActionResult> ThreadMessages(Guid threadId)
        {
            var messages = await _ms.GetThreadMessages(User, threadId);

            return PartialView("_ThreadMessagesPartial", messages);
        }

        // GET: Messages/UnreadCount
        public async Task<ActionResult> UnreadCount()
        {
            var count = await _ms.GetUnreadCount(User);

            return Json(count, JsonRequestBehavior.AllowGet);
        }

        // GET: Messages/Create
        public async Task<ActionResult> Create(int orderId)
        {
            var orderMessage = await Db.Messages.FirstOrDefaultAsync(x => x.Order.Id == orderId && x.Status != MessageStatus.Deleted);
            if (orderMessage != null) return RedirectToAction("Details", new {id = orderMessage.Id});

            var messageModel = await GetMessageModelForCreation(orderId);
            if (messageModel == null) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View("Create", messageModel);
        }
        
        // POST: Admin/Messages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MessageModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var messageModel = await GetMessageModelForCreation(model.Order.Id);
            if (messageModel == null) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            messageModel.Content = model.Content;
            
            await _ms.Send(User, messageModel);
            return RedirectToAction("Details", "Messages", new {id = messageModel.Id});
        }

        private async Task<MessageModel> GetMessageModelForCreation(int orderId)
        {
            var order = await Db.OrderModels.Include(x => x.OrderAmbassador).Include(x => x.OrderRequestor)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null || !order.CanView(User)) return null;

            var userId = User.Identity.GetUserId();
            var fromUser = Db.Users.Find(userId);
            var toUser = Db.Users.Find(User.IsInRole(AppRoles.Requester)
                ? order.OrderAmbassador.UserId
                : order.OrderRequestor.UserId);

            var messageModel = new MessageModel
            {
                From = fromUser,
                To = toUser,
                Order = order
            };
            return messageModel;
        }

        // GET: Messages/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var messageModel = await _ms.View(User, id.Value);
            if (messageModel == null)
            {
                return HttpNotFound();
            }

            return View(messageModel);
        }

        // GET: Messages/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var messageModel = await _ms.View(User, id.Value);
            if (messageModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReturnUrl = Request.UrlReferrer?.PathAndQuery;
            return View(messageModel);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id, string returnUrl)
        {
            await _ms.Delete(User, id);

            return string.IsNullOrWhiteSpace(returnUrl) ? 
                            (ActionResult) RedirectToAction("Index") :
                            Redirect(returnUrl);
        }

        // POST: Messages/Reply/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reply(MessageModel messageModel)
        {
            var userId = User.Identity.GetUserId();
            var fromUser = Db.Users.Find(userId);
            messageModel.From = fromUser;

            var mainMessage = await _ms.View(User, messageModel.PreviousMessage.Id);
            messageModel.PreviousMessage = mainMessage;
            
            await _ms.Send(User, messageModel);

            if (!_connections.IsUserOnline(messageModel.To.Email))
            {
                NotifyUserChat data = new NotifyUserChat
                {
                    ChatUrl = HttpContext.Request.UrlReferrer.ToString(),
                    FromEmail = messageModel.From.Email,
                    ToEmail = messageModel.To.Email
                };

                //var mailMessage = new MailMessage
                //{
                //    From = _fromEmail,
                //    Subject = $"[Atomic Limbs] Tenés un mensaje nuevo de {messageModel.From.Email}",
                //    To = messageModel.To.Email,
                //    Body = CompiledTemplateEngine.Render("Mails.NotifyUserMessage", data)
                //};

                //await AzureQueue.EnqueueAsync(mailMessage);
            }

            return PartialView("_Detail", messageModel);
        }

        [HttpPost]
        public async Task MarkAsRead(Guid id)
        {
            await _ms.View(User, id);
        }
    }
}