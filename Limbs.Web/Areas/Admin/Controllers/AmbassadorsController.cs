using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;
using Limbs.Web.Helpers;
using Microsoft.AspNet.Identity;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class AmbassadorsController : AdminBaseController
    {
        // GET: Admin/Ambassadors
        public ActionResult Index()
        {
            return View(Db.AmbassadorModels.ToList());
        }
        
        // GET: Admin/Ambassador/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ambassadorModel = await Db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }
            return View(ambassadorModel);
        }


        // GET: Admin/Ambassador/Edit/5
        [OverrideAuthorize(Roles = AppRoles.Administrator + "," + AppRoles.Ambassador)]
        public async Task<ActionResult> Edit(int? id)
        {
            AmbassadorModel ambassadorModel;
            var userId = User.Identity.GetUserId();
            if (!id.HasValue && User.IsInRole(AppRoles.Ambassador))
            {
                ambassadorModel = await Db.AmbassadorModels.FirstAsync(x => x.UserId == userId);
            }
            else
            {
                if(!id.HasValue) return HttpNotFound();
                ambassadorModel = await Db.AmbassadorModels.FindAsync(id);
            }
            if (!CanViewOrEdit(userId, ambassadorModel))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            ViewBag.ReturnUrl = Request.UrlReferrer.AbsoluteUri;
            return View(ambassadorModel);
        }
        
        private bool CanViewOrEdit(string userId, AmbassadorModel ambassadorModel)
        {
            if (User.IsInRole(AppRoles.Administrator))
            {
                return true;
            }

            return ambassadorModel.UserId == userId;
        }

        // POST: Admin/Ambassador/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OverrideAuthorize(Roles = AppRoles.Administrator + "," + AppRoles.Ambassador)]
        public async Task<ActionResult> Edit(AmbassadorModel ambassadorModel, string returnUrl)
        {
            if (!ModelState.IsValid) return View(ambassadorModel);

            var pointAddress = ambassadorModel.Country + ", " + ambassadorModel.City + ", " + ambassadorModel.Address;
            ambassadorModel.Location = Geolocalization.GetPoint(pointAddress);

            if (!User.IsInRole(AppRoles.Administrator))
            {
                ambassadorModel.Email = User.Identity.GetUserName();
            }
            if (!CanViewOrEdit(User.Identity.GetUserId(), ambassadorModel))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }

            Db.Entry(ambassadorModel).State = EntityState.Modified;
            
            await Db.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        // GET: Admin/Ambassador/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ambassadorModel = await Db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }
            return View(ambassadorModel);
        }

        // POST: Admin/Ambassador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var ambassadorModel = await Db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }

            Db.AmbassadorModels.Remove(ambassadorModel);
            await Db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}