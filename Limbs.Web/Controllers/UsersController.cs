using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Common.Geocoder;
using Limbs.Web.Common.Geocoder.Google;
using Limbs.Web.Entities.Models;
using Microsoft.AspNet.Identity;
using Limbs.Web.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Limbs.Web.Controllers
{
    [DefaultAuthorize(Roles = AppRoles.Requester)]
    public class UsersController : BaseController
    {
        // GET: Users
        public ActionResult Index(string message)
        {
            var userId = User.Identity.GetUserId();
            var orderList = Db.OrderModels.Where(c => c.OrderRequestor.UserId == userId).ToList();
            var user = Db.UserModelsT.SingleOrDefault(u => u.UserId == userId);

            if (user == null) return RedirectToAction("Login", "Account");

            if (!orderList.Any() && user.IsValidAge()) return RedirectToAction("ManoPedir", "Orders");

            var viewModel = new UserPanelViewModel
            {
                Order = orderList.ToList(),
                Message = message,
                IsValidAge = user.IsValidAge(),
                UserName = user.Name,

            };

            return View(viewModel);
        }

        // GET: Users/TermsAndConditions
        [OverrideAuthorize(Roles = AppRoles.Unassigned + "," + AppRoles.Requester)]
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        // GET: Users/Create
        [OverrideAuthorize(Roles = AppRoles.Unassigned)]
        public ActionResult Create()
        {
            return View();
        }

        // GET: Users/Edit
        [OverrideAuthorize(Roles = AppRoles.Requester + "," + AppRoles.Administrator)]
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
            if (userModel == null || !userModel.CanViewOrEdit(User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            ViewBag.IsAdultCheck = true;
            ViewBag.ReturnUrl = Request.UrlReferrer?.AbsoluteUri;
            return View("Create", userModel);
        }

        // POST: Users/Create
        [HttpPost]
        [OverrideAuthorize(Roles = AppRoles.Unassigned + "," + AppRoles.Requester + "," + AppRoles.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserModel userModel, bool isAdultCheck, bool? termsAndConditions)
        {
            if (!User.IsInRole(AppRoles.Administrator))
            {
                userModel.Email = User.Identity.GetUserName();
                userModel.UserId = User.Identity.GetUserId();
            }

            await ValidateUserModel(userModel, isAdultCheck, termsAndConditions);
            ViewBag.IsAdultCheck = isAdultCheck;
            ViewBag.TermsAndConditions = termsAndConditions;
            if (!ModelState.IsValid) return View("Create", userModel);

            var user = await Db.UserModelsT.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userModel.Id);
            if (user == null)
            {
                //CREATE
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Db));
                await userManager.RemoveFromRoleAsync(userModel.UserId, AppRoles.Unassigned);
                await userManager.AddToRoleAsync(userModel.UserId, AppRoles.Requester);

                userModel.RegisteredAt = DateTime.UtcNow;

                Db.UserModelsT.Add(userModel);
                await Db.SaveChangesAsync();


                return RedirectToAction("Index");
            }
            if (!userModel.CanViewOrEdit(User)) return new HttpStatusCodeResult(HttpStatusCode.Conflict);

            if (!user.CanViewOrEdit(User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            //EDIT
            Db.Entry(userModel).State = EntityState.Modified;
            await Db.SaveChangesAsync();

            return RedirectToAction("Index", "Manage");
        }

        private async Task ValidateUserModel(UserModel userModel, bool isAdultCheck, bool? termsAndConditions)
        {
            ModelState[nameof(userModel.Id)]?.Errors.Clear();
            ModelState[nameof(userModel.UserId)]?.Errors.Clear();
            ModelState[nameof(userModel.Email)]?.Errors.Clear();

            userModel.Location = GeocoderLocation.GeneratePoint(userModel.LatLng.Split(','));
            /*
            ModelState[nameof(userModel.Location)]?.Errors.Clear();
            
            var pointAddress = userModel.Country + ", " + userModel.State + ", " + userModel.City + ", " + userModel.Address;
            var address = await GeocoderLocation.GetAddressAsync(pointAddress) as GoogleAddress;
            userModel.Location = GeocoderLocation.GeneratePoint(address);

            if (address == null)
                ModelState.AddModelError(nameof(userModel.Address), @"Dirección inválida.");

            if (address != null && address[GoogleAddressType.StreetNumber] == null)
                ModelState.AddModelError(nameof(userModel.Address), @"La dirección debe tener altura en la calle.");
            */

            if (termsAndConditions.HasValue && !termsAndConditions.Value)
                ModelState.AddModelError(nameof(termsAndConditions), @"Debe aceptar terminos y condiciones.");

            if (userModel.IsProductUser)
            {
                ModelState[nameof(userModel.ResponsableName)].Errors.Clear();
                ModelState[nameof(userModel.ResponsableLastName)].Errors.Clear();
                ModelState[nameof(userModel.ResponsableDni)].Errors.Clear();
                if (userModel.Birth >= DateTime.UtcNow.AddYears(-18))
                    ModelState.AddModelError(nameof(userModel.Birth), @"Debe ser mayor de 18 años.");
                return;
            }

            if (!isAdultCheck)
                ModelState.AddModelError("BirthDeclaration", @"Debe ser mayor de 18 años.");

            //if (userModel.Birth >= DateTime.UtcNow.AddYears(-4))
            //    ModelState.AddModelError(nameof(userModel.Birth), @"El usuario de la prótesis debe ser mayor de 4 años.");

            if (string.IsNullOrWhiteSpace(userModel.ResponsableName))
                ModelState.AddModelError(nameof(userModel.ResponsableName), @" ");

            if (string.IsNullOrWhiteSpace(userModel.ResponsableLastName))
                ModelState.AddModelError(nameof(userModel.ResponsableLastName), @" ");

            if (string.IsNullOrWhiteSpace(userModel.ResponsableDni))
                ModelState.AddModelError(nameof(userModel.ResponsableDni), @" ");
        }
    }
}
