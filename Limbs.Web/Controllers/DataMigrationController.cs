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
using System.Globalization;

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
        public async Task<string> MigrateUsers(HttpPostedFileBase postedFile)
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

                Response.Write(" \n<br />DATA VALIDATION \n");

                foreach (string row in System.IO.File.ReadLines(filePath).Skip(1))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        try
                        {
                            var strings = row.Split('\t');
                            var email = strings[0].Clean();
                            var pUser = Db.UserModelsT.FirstOrDefault(x => x.Email.Equals(email));
                            if (pUser != null) continue;
                        
                            var userModel = new UserModel
                            {
                                Email = strings[0].Clean(),
                                IsProductUser = bool.Parse(strings[1].Clean()),
                                UserName = strings[2].Clean(),
                                UserLastName = string.IsNullOrWhiteSpace(strings[3].Clean()) ? "-" : strings[3].Clean(),
                                ResponsableName = strings[4].Clean(),
                                ResponsableLastName = strings[5].Clean(),
                                Phone = string.IsNullOrWhiteSpace(strings[6].Clean()) ? "-" : strings[6].Clean(),
                                Birth = DateTime.ParseExact(strings[7].Clean(), "d/M/yyyy", CultureInfo.InvariantCulture),
                                Gender = "hombre".Equals(strings[8].Clean()) ? Gender.Hombre : Gender.Mujer,
                                Country = strings[9].Clean(),
                                City = string.IsNullOrWhiteSpace(strings[10].Clean()) ? "-" : strings[10].Clean(),
                                Address = string.IsNullOrWhiteSpace(strings[11].Clean()) ? "-" : strings[11].Clean(),
                                Dni = string.IsNullOrWhiteSpace(strings[12].Clean()) ? "-" : strings[12].Clean(),
                                RegisteredAt = DateTime.ParseExact(strings[13].Clean(), "d/M/yyyy H:mm:ss", CultureInfo.InvariantCulture),
                                UserId = strings[0].Clean(),
                                State = "-",
                                Address2 = "-",
                                LatLng = "0.1,0.1",
                            };

                            users.Add(userModel);
                        }
                        catch (Exception e)
                        {
                            Response.Write(" \n<br />ER: " + row);
                            Response.Write(e.Message);
                        }
                    }
                }

                Response.Write(" \n<br />-------------- \n");
                Response.Write(" \n<br />DATA MIGRATION \n");

                foreach (var user in users)
                {
                    try
                    {
                        var password = Membership.GeneratePassword(8, 1) + "1";
                        var newUser = new ApplicationUser { UserName = user.Email, Email = user.Email };
                        var result = await UserManager.CreateAsync(newUser, password);
                        if (result.Succeeded)
                        {
                            await UserManager.AddToRoleAsync(newUser.Id, AppRoles.Requester);

                            user.UserId = newUser.Id;
                            Db.UserModelsT.Add(user);
                            await Db.SaveChangesAsync();

                            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.UserId);
                            await UserManager.ConfirmEmailAsync(user.UserId, token);
                            await Db.SaveChangesAsync();
                 
                            await SendEmailConfirmation(newUser);

                            Response.Write(" \n<br />OK: " + user.Email);
                        }
                        else
                        {
                            Response.Write(" \n<br />ER: " + user.Email);
                        }
                    }
                    catch (Exception e)
                    {
                        Response.Write(" \n<br />ER: " + user.Email);
                        Response.Write(e.Message);
                    }
                }
            }
            System.IO.File.Delete(filePath);
            Response.End();
            return string.Empty;
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
