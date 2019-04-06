using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;
using Limbs.Web.Entities.WebModels.Admin.Models;
using Limbs.Web.Entities.WebModels.Extensions;
using Limbs.Web.Logic.Services;
using Microsoft.AspNet.Identity;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class MessagesController : AdminBaseController
    {
        private readonly IMessageService _ms;

        public MessagesController(IMessageService ms)
        {
            _ms = new MessagesService(Db);
        }

        // GET: Admin/Messages
        public async Task<ActionResult> Index()
        {
            return View(await _ms.GetAllMessages(User));
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
                var parentMessage = await _ms.View(User, parentId.Value);
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
            else
            {
                ViewBag.Users = Db.Users.ToList().Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Email,
                });
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
                //PreviousMessage = messageViewModel.PreviousMessage,
                Content = messageViewModel.Content,
                //Order = messageViewModel.Order,
            };

            await _ms.Send(User, messageModel);

            return RedirectToAction("Index");
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
