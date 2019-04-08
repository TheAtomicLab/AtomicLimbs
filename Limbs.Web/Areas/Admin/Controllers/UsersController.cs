using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;
using Limbs.Web.Entities.WebModels.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class UsersController : AdminBaseController
    {
        private readonly RoleManager<IdentityRole> _roleMngr;
        public UsersController()
        {
            var roleStore = new RoleStore<IdentityRole>(Db);
            _roleMngr = new RoleManager<IdentityRole>(roleStore);
        }

        // GET: Admin/Users
        public ActionResult Index()
        {
            //TODO (ale): implementar paginacion, export to excel, ordenamiento
            var userList = Db.UserModelsT.ToList();
            return View(userList);
        }

        // GET: Admin/Users/Registered
        public ActionResult Registered()
        {
            //TODO (ale): implementar paginacion, export to excel, ordenamiento
            var userList = Db.Users.Include(x => x.Roles).ToList();
            var ar = _roleMngr.Roles.ToList();

            var result = userList.Select(user =>
                {
                    var roles = user.Roles.Select(x => x.RoleId).Select(r => ar.FirstOrDefault(x => x.Id == r)?.Name).ToList();
                    if (ConfigurationManager.AppSettings["AdminEmails"].Split(',').ToList().Contains(user.Email)) roles.Add(AppRoles.Administrator);
                    
                    return new UserViewModel
                    {
                        Email = user.Email,
                        IsEmailConfirmed = user.EmailConfirmed,
                        Roles = roles.ToArray(),
                        LoginProviders = user.Logins.Select(x => x.LoginProvider).ToArray(),
                    };
                })
                .ToList();
            
            return View(result);
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