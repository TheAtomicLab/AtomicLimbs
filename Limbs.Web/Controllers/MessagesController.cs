using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;
using Limbs.Web.Services;

namespace Limbs.Web.Controllers
{
    [DefaultAuthorize(Roles = AppRoles.User + "," + AppRoles.Administrator)]
    public class MessagesController : BaseController
    {

        private readonly IMessageService _ms;
        public MessagesController(IMessageService ms)
        {
            _ms = new MessagesService(Db);
        }

        // GET: Messages
        public ActionResult Index()
        {
            return RedirectToAction("Inbox");
        }

        // GET: Messages/Inbox
        public async Task<ActionResult> Inbox()
        {
            var messages = await _ms.GetInboxMessages(User);

            return View(messages);
        }

        public async Task<ActionResult> InboxPartial()
        {
            var messages = await _ms.GetInboxMessages(User);

            return PartialView("_InboxPartial", messages);
        }
        
        // GET: Messages/UnreadCount
        public async Task<ActionResult> UnreadCount()
        {
            var count = await _ms.GetUnreadCount(User);

            return Json(count, JsonRequestBehavior.AllowGet);
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
            return View(messageModel);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            await _ms.Delete(User, id);

            return RedirectToAction("Index");
        }

        public ActionResult Reply(Guid id)
        {
            return RedirectToAction("Create", new { parentId = id });
        }
    }
}