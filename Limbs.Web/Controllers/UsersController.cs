using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;
using Microsoft.AspNet.Identity;
using Limbs.Web.Helpers;
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

            var viewModel = new UserPanelViewModel
            {
                Order = orderList.ToList(),
                Message = message

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

            ValidateUserModel(userModel, isAdultCheck, termsAndConditions);
            ViewBag.IsAdultCheck = isAdultCheck;
            ViewBag.TermsAndConditions = termsAndConditions;
            if (!ModelState.IsValid) return View("Create", userModel);
            
            var pointAddress = userModel.Country + ", " + userModel.City + ", " + userModel.Address;
            userModel.Location = await Geolocalization.GetPointAsync(pointAddress);

            if (userModel.Id == 0)
            {
                //CREATE
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Db));
                await userManager.RemoveFromRoleAsync(userModel.UserId, AppRoles.Unassigned);
                await userManager.AddToRoleAsync(userModel.UserId, AppRoles.Requester);

                Db.UserModelsT.Add(userModel);
                await Db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            if (!userModel.CanViewOrEdit(User)) return new HttpStatusCodeResult(HttpStatusCode.Conflict);

            //EDIT
            Db.Entry(userModel).State = EntityState.Modified;
            await Db.SaveChangesAsync();
            
            return RedirectToAction("Index", "Manage");
        }

        private void ValidateUserModel(UserModel userModel, bool isAdultCheck, bool? termsAndConditions)
        {
            ModelState[nameof(userModel.Id)]?.Errors.Clear();
            ModelState[nameof(userModel.UserId)]?.Errors.Clear();
            ModelState[nameof(userModel.Email)]?.Errors.Clear();

            if (termsAndConditions.HasValue && !termsAndConditions.Value)
                ModelState.AddModelError(nameof(termsAndConditions), @"Debe aceptar terminos y condiciones.");
            
            if (userModel.IsProductUser)
            {
                ModelState[nameof(userModel.ResponsableName)].Errors.Clear();
                ModelState[nameof(userModel.ResponsableLastName)].Errors.Clear();
                if (userModel.Birth >= DateTime.UtcNow.AddYears(-18))
                    ModelState.AddModelError(nameof(userModel.Birth), @"Debe ser mayor de 18 años.");
                return;
            }

            if (!isAdultCheck)
                ModelState.AddModelError("BirthDeclaration", @"Debe ser mayor de 18 años.");

            if (userModel.Birth >= DateTime.UtcNow.AddYears(-4))
                ModelState.AddModelError(nameof(userModel.Birth), @"El usuario de la mano debe ser mayor de 4 años.");

            if (string.IsNullOrWhiteSpace(userModel.ResponsableName))
                ModelState.AddModelError(nameof(userModel.ResponsableName), @" ");

            if (string.IsNullOrWhiteSpace(userModel.ResponsableLastName))
                ModelState.AddModelError(nameof(userModel.ResponsableLastName), @" ");
        }
    }
}
