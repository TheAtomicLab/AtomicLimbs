using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Limbs.Web.Areas.Admin.Models;
using Limbs.Web.Entities.Models;
using Limbs.Web.Extensions;
using Microsoft.AspNet.Identity;
using MessageModel = Limbs.Web.Entities.Models.MessageModel;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class MessagesController : AdminBaseController
    {
        // GET: Admin/Messages
        public async Task<ActionResult> Index()
        {
            return View(await Db.Messages.ToListAsync());
        }

        // GET: Admin/Messages/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MessageModel messageModel = await Db.Messages.FindAsync(id);
            if (messageModel == null)
            {
                return HttpNotFound();
            }
            return View(messageModel);
        }

        // GET: Admin/Messages/Create
        public async Task<ActionResult> Create(Guid? parentId, Guid? to)
        {
            var userId = User.Identity.GetUserId();
            var fromUser = Db.Users.Find(userId);

            var messageModel = new MessageViewModel();
            messageModel.AddFrom(fromUser);
            if (parentId.HasValue)
            {
                var parentMessage = await Db.Messages.FindAsync(parentId.Value);
                messageModel.PreviousMessage = parentMessage;
                if (parentMessage != null)
                {
                    messageModel.Priority = parentMessage.Priority;
                    if (messageModel.From.Equals(parentMessage.From)) //adding to your message
                    {
                        messageModel.To = parentMessage.To.ToViewModel();
                    }
                    else
                    {
                        messageModel.To = parentMessage.From.ToViewModel();
                    }
                }
            }
            if (to.HasValue)
            {
                var toId = to.Value.ToString("D");
                var toUser = Db.Users.FirstOrDefault(x => x.Id == toId);
                messageModel.To = toUser.ToViewModel();
            }

            return View("Create", messageModel);
        }

        // POST: Admin/Messages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MessageViewModel messageViewModel)
        {
            if (!ModelState.IsValid) return View(messageViewModel);
            
            var fromUser = Db.Users.FirstOrDefault(x => x.Id == messageViewModel.From.Id);
            var toUser = Db.Users.FirstOrDefault(x => x.Id == messageViewModel.To.Id);

            var messageModel = new MessageModel
            {
                From = fromUser,
                To = toUser,
                Id = Guid.NewGuid(),
                Priority = messageViewModel.Priority,
                PreviousMessage = messageViewModel.PreviousMessage,
                Content = messageViewModel.Content,
                Order = messageViewModel.Order,
            };

            Db.Messages.Add(messageModel);
            await Db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        
        // GET: Admin/Messages/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var messageModel = await Db.Messages.FindAsync(id);
            if (messageModel == null)
            {
                return HttpNotFound();
            }
            return View(messageModel);
        }

        // POST: Admin/Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var messageModel = await Db.Messages.FindAsync(id);

            if (messageModel != null) messageModel.Status = MessageStatus.Deleted;

            await Db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public ActionResult Reply(Guid id)
        {
            return RedirectToAction("Create", new { parentId = id });
        }

        public ActionResult GetUsername(string term)
        {
            ApplicationUser[] matching = string.IsNullOrWhiteSpace(term) ?
                Db.Users.ToArray() :
                Db.Users.Where(p => p.Email.ToUpper().StartsWith(term.ToUpper())).ToArray();

            return Json(matching.Select(m => new {
                id = m.Id,
                value = m.Email,
                label = m.UserName.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
    }
}
