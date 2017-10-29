using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;
using Limbs.Web.Helpers;
using Microsoft.AspNet.Identity;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class UsersController : AdminBaseController
    {
        // GET: Admin/Users
        public ActionResult Index()
        {
            //TODO (ale): implementar paginacion, export to excel, ordenamiento
            var userList = Db.UserModelsT.ToList();
            return View(userList);
        }

        // GET: Admin/Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userModel = await Db.UserModelsT.FindAsync(id);
            if (userModel == null)
            {
                return HttpNotFound();
            }
            return View(userModel);
        }

        // GET: Admin/Users/Edit/5
        [OverrideAuthorize(Roles = AppRoles.Administrator + "," + AppRoles.Requester)]
        public async Task<ActionResult> Edit(int? id)
        {
            UserModel userModel;
            var userId = User.Identity.GetUserId();
            if (!id.HasValue && User.IsInRole(AppRoles.Requester))
            {
                userModel = await Db.UserModelsT.FirstAsync(x => x.UserId == userId);
            }
            else
            {
                if (!id.HasValue) return HttpNotFound();
                userModel = await Db.UserModelsT.FindAsync(id);
            }
            if (!CanViewOrEdit(userId, userModel))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            ViewBag.ReturnUrl = Request.UrlReferrer.AbsoluteUri;
            return View(userModel);
        }

        private bool CanViewOrEdit(string userId, UserModel userModel)
        {
            if (User.IsInRole(AppRoles.Administrator))
            {
                return true;
            }

            return userModel.UserId == userId;
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OverrideAuthorize(Roles = AppRoles.Administrator + "," + AppRoles.Requester)]
        public async Task<ActionResult> Edit(UserModel userModel, string returnUrl)
        {
            if (!ModelState.IsValid) return View(userModel);

            var pointAddress = userModel.Country + ", " + userModel.City + ", " + userModel.Address;
            userModel.Location = Geolocalization.GetPoint(pointAddress);

            if (!User.IsInRole(AppRoles.Administrator))
            {
                userModel.Email = User.Identity.GetUserName();
            }
            if (!CanViewOrEdit(User.Identity.GetUserId(), userModel))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }

            Db.Entry(userModel).State = EntityState.Modified;

            await Db.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        // GET: Admin/Users/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userModel = await Db.UserModelsT.FindAsync(id);
            if (userModel == null)
            {
                return HttpNotFound();
            }
            return View(userModel);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var userModel = await Db.UserModelsT.FindAsync(id);

            if (userModel == null)
            {
                return HttpNotFound();
            }

            Db.UserModelsT.Remove(userModel);
            await Db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}