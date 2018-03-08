using Limbs.Web.Entities.Models;
using Limbs.Web.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace Limbs.Web.Controllers
{
    [DefaultAuthorize(Roles = AppRoles.Administrator)]
    public class DataMigrationController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ActionResult Index()
        {
            return View("UserMigration", new List<MigrateUsersViewModel>());
        }

        [HttpPost]
        public async Task<ActionResult> MigrateUsers(HttpPostedFileBase postedFile)
        {
            List<MigrateUsersViewModel> users = new List<MigrateUsersViewModel>();
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string csvData = System.IO.File.ReadAllText(filePath);
                foreach (string row in csvData.Split('\r'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        users.Add(new MigrateUsersViewModel
                        {
                            Email = row.Split(';')[0],
                            Password = row.Split(';')[1]
                        });
                    }
                }

                foreach (var user in users)
                {
                    var newUser = new ApplicationUser { UserName = user.Email, Email = user.Email };
                    var result = await UserManager.CreateAsync(newUser, user.Password);
                    if (result.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(newUser.Id, AppRoles.Requester);

                  //      await SendEmailConfirmation(newUser);

                    }
                }

            }
            return View("UserMigration", users);
        }
    }
}