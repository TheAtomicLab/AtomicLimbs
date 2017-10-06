using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;

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
        public async Task<ActionResult> Edit(int? id)
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

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserModel userModel)
        {
            if (!ModelState.IsValid) return View(userModel);

            Db.Entry(userModel).State = EntityState.Modified;
            await Db.SaveChangesAsync();

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