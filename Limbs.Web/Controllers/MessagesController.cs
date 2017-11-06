using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;
using Microsoft.AspNet.Identity;

namespace Limbs.Web.Controllers
{
    [DefaultAuthorize(Roles = AppRoles.User + "," + AppRoles.Administrator)]
    public class MessagesController : BaseController
    {
        // GET: Messages
        public ActionResult Index()
        {
            return RedirectToAction("Inbox");
        }

        // GET: Messages
        public async Task<ActionResult> Inbox()
        {
            var userId = User.Identity.GetUserId();

            var messages = await Db.Messages.Include(x => x.From).Where(x => x.To.Id == userId && x.Status != MessageStatus.Deleted).ToListAsync();

            return View(messages);
        }

        // GET: Admin/Messages/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var messageModel = await Db.Messages.Include(x => x.To).FirstOrDefaultAsync(x => x.Id == id);
            if (messageModel == null)
            {
                return HttpNotFound();
            }

            if (messageModel.Status == MessageStatus.Unread)
            {
                var userId = User.Identity.GetUserId();
                if (messageModel.To.Id == userId)
                {
                    messageModel.Status = MessageStatus.Read;

                    await Db.SaveChangesAsync();
                }
            }

            return View(messageModel);
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
    }
}