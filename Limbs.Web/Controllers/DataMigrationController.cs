using Limbs.Web.Entities.Models;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System;
using Limbs.Web.Common.Mail;
using System.Linq;
using System.Web.Security;

namespace Limbs.Web.Controllers
{
    [DefaultAuthorize(Roles = AppRoles.Administrator)]
    public class DataMigrationController : BaseController
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ActionResult Index()
        {
            return View("UserMigration", new List<UserModel>());
        }

        [HttpPost]
        public async Task<ActionResult> MigrateUsers(HttpPostedFileBase postedFile)
        {
            List<UserModel> users = new List<UserModel>();
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
                foreach (string row in csvData.Split('\r').Skip(1))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var email = row.Split(';')[0].Clean();
                        var pUser = Db.UserModelsT.FirstOrDefault(x => x.Email.Equals(email));
                        if (pUser != null) continue;

                        users.Add(new UserModel
                        {
                            Email = row.Split(';')[0].Clean(),
                            IsProductUser = bool.Parse(row.Split(';')[1].Clean()),
                            UserName = row.Split(';')[2].Clean(),
                            UserLastName = row.Split(';')[3].Clean(),
                            ResponsableName = row.Split(';')[4].Clean(),
                            ResponsableLastName = row.Split(';')[5].Clean(),
                            Phone = string.IsNullOrWhiteSpace(row.Split(';')[6].Clean()) ? "-" : row.Split(';')[6].Clean(),
                            Birth = DateTime.Parse(row.Split(';')[7].Clean()),
                            Gender = "hombre".Equals(row.Split(';')[8].Clean()) ? Gender.Hombre : Gender.Mujer,
                            Country = row.Split(';')[9].Clean(),
                            City = string.IsNullOrWhiteSpace(row.Split(';')[10].Clean()) ? "-" : row.Split(';')[10].Clean(),
                            Address = string.IsNullOrWhiteSpace(row.Split(';')[11].Clean()) ? "-" : row.Split(';')[11].Clean(),
                            Dni = string.IsNullOrWhiteSpace(row.Split(';')[12].Clean()) ? "-" : row.Split(';')[12].Clean(),
                            RegisteredAt = DateTime.Parse(row.Split(';')[13].Clean()),
                            UserId = row.Split(';')[0].Clean(),
                            State = "-",
                            Address2 = "-",
                            LatLng = "0.1,0.1",
                        });
                    }
                }

                foreach (var user in users)
                {
                    var password = Membership.GeneratePassword(8, 1) + "1";
                    var newUser = new ApplicationUser { UserName = user.Email, Email = user.Email };
                    var result = await UserManager.CreateAsync(newUser, password);
                    if (result.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(newUser.Id, AppRoles.Requester);
                    }

                    user.UserId = newUser.Id;
                    
                    Db.UserModelsT.Add(user);
                    await Db.SaveChangesAsync();

                    await SendEmailConfirmation(newUser);
                }


            }
            System.IO.File.Delete(filePath);
            return View("UserMigration", users);
        }

        private async Task SendEmailConfirmation(ApplicationUser user)
        {
            if (Request.Url != null)
            {
                //TODO: token time.
                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, Request.Url.Scheme);
                var body = CompiledTemplateEngine.Render("Mails.EmailPasswordChangeImport", callbackUrl);

                await UserManager.SendEmailAsync(user.Id, "[Acción Requerida] ¡Te presentamos la nueva plataforma! Continuá con tu pedido ahora para obtener tu mano 3D", body);
            }
        }

    }

    public static class StringExtensions
    {

        public static string Clean(this string source)
        {
            return source.Trim().ToLowerInvariant();
        }
    }
}
