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
using System.Text;
using Microsoft.AspNet.Identity;

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
        public string MigrateUsers(HttpPostedFileBase postedFile)
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
                postedFile.SaveAs(filePath);

                Response.Write(" \n<br />DATA VALIDATION \n");

                foreach (string row in System.IO.File.ReadLines(filePath, Encoding.ASCII).Skip(1))
                {
                    if (!string.IsNullOrWhiteSpace(row))
                    {
                        var strings = row.Split('\t');
                        try
                        {
                            var email = strings[0].Clean();
                            var pUser = Db.UserModelsT.FirstOrDefault(x => x.Email.Equals(email));
                            if (pUser != null)
                            {
                                Response.Write(" \n<br />SKIP: " + pUser.Email);
                                continue;
                            }

                            var birth = GetDateTimeFromText(strings[7].Clean());
                            var registeredAt = GetDateTimeFromText(strings[13].Clean());

                            var userModel = new UserModel
                            {
                                Email = strings[0].Clean(),
                                IsProductUser = bool.Parse(strings[1].Clean()),
                                UserName = strings[2].Clean(),
                                UserLastName = strings[3].Clean(),
                                ResponsableName = strings[4].Clean(),
                                ResponsableLastName = strings[5].Clean(),
                                Phone = string.IsNullOrWhiteSpace(strings[6].Clean()) ? "-" : strings[6].Clean(),
                                Birth = birth,
                                Gender = "hombre".Equals(strings[8].Clean()) ? Gender.Hombre : Gender.Mujer,
                                Country = strings[9].Clean(),
                                City = string.IsNullOrWhiteSpace(strings[10].Clean()) ? "-" : strings[10].Clean(),
                                Address = string.IsNullOrWhiteSpace(strings[11].Clean()) ? "-" : strings[11].Clean(),
                                Dni = string.IsNullOrWhiteSpace(strings[12].Clean()) ? "-" : strings[12].Clean(),
                                RegisteredAt = registeredAt,
                                UserId = strings[0].Clean(),
                                State = "-",
                                Address2 = "-",
                                LatLng = "0.1,0.1",
                            };

                            users.Add(userModel);
                            Response.Write(" \n<br />OK: " + userModel.Email);
                        }
                        catch (Exception e)
                        {
                            Response.Write(" \n<br />ER: " + row);
                            Response.Write(strings[1]);
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
                        var result = UserManager.CreateAsync(newUser, password).Result;
                        if (result.Succeeded)
                        {
                            UserManager.AddToRoleAsync(newUser.Id, AppRoles.Requester).Wait();

                            var code = UserManager.GenerateEmailConfirmationTokenAsync(newUser.Id).Result;
                            UserManager.ConfirmEmailAsync(newUser.Id, code).Wait();


                            user.UserId = newUser.Id;
                            newUser.EmailConfirmed = true;
                            Db.UserModelsT.Add(user);
                            Db.SaveChanges();

                            SendEmailConfirmation(newUser).Wait();
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
                        Response.Write($"{e.Message} {e.InnerException?.Message} {e.InnerException?.InnerException?.Message}");
                        Response.Write("||");
                    }
                }
            }
            System.IO.File.Delete(filePath);
            Response.End();
            return string.Empty;
        }

        private static DateTime GetDateTimeFromText(string input)
        {
            if (DateTime.TryParseExact(input, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var r)) return r;
            if (DateTime.TryParseExact(input, "d/M/yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var t)) return t;
            if (DateTime.TryParseExact(input, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var q)) return q;
            return DateTime.TryParseExact(input, "M/d/yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out r) 
                ? r 
                : new DateTime(2000, 1, 1);
        }

        private Task SendEmailConfirmation(ApplicationUser user)
        {
            if (Request.Url != null)
            {
                //TODO: token time.
                var code = UserManager.GeneratePasswordResetTokenAsync(user.Id).Result;
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, Request.Url.Scheme);
                var body = CompiledTemplateEngine.Render("Mails.EmailPasswordChangeImport", callbackUrl);

                return UserManager.SendEmailAsync(user.Id, "[Acción Requerida] ¡Te presentamos la nueva plataforma! Continuá con tu pedido ahora para obtener tu prótesis impresa en 3D", body);
            }
            return Task.CompletedTask;
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
