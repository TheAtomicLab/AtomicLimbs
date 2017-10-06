using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;
using Limbs.Web.Models;
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
        [OverrideAuthorize(Roles = AppRoles.Unassigned)]
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

        // POST: Users/Create
        [HttpPost]
        [OverrideAuthorize(Roles = AppRoles.Unassigned)]
        public async Task<ActionResult> Create(UserModel userModel, bool selectUser, bool isAdultCheck)
        {
            ViewBag.SelectUser = selectUser;
            ViewBag.IsAdultCheck = isAdultCheck;

            ValidateUserModel(userModel, selectUser, isAdultCheck);
            if (!ModelState.IsValid) return View("Create", userModel);
            
            var pointAddress = userModel.Country + ", " + userModel.City + ", " + userModel.Address;

            userModel.Location = Geolocalization.GetPoint(pointAddress);
            userModel.Email = User.Identity.GetUserName();
            userModel.UserId = User.Identity.GetUserId();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Db));
            await userManager.RemoveFromRoleAsync(userModel.UserId, AppRoles.Unassigned);
            await userManager.AddToRoleAsync(userModel.UserId, AppRoles.Requester);

            Db.UserModelsT.Add(userModel);
            await Db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private void ValidateUserModel(UserModel userModel, bool selectUser, bool isAdultCheck)
        {
            if (selectUser)
            {
                userModel.UserName = userModel.ResponsableName;
                userModel.UserLastName = userModel.ResponsableLastName;
                ModelState[nameof(userModel.UserName)].Errors.Clear();
                ModelState[nameof(userModel.UserLastName)].Errors.Clear();
                if (userModel.Birth >= DateTime.UtcNow.AddYears(-18))
                {
                    ModelState.AddModelError("minbirthday", "Debe ser mayor de 18 años.");
                }
                return;
            }
            if(!isAdultCheck)
                ModelState.AddModelError("minbirthdayresponsable", "Debe ser mayor de 18 años.");

            if (userModel.Birth >= DateTime.UtcNow.AddYears(-4))
            {
                ModelState.AddModelError("minbirthday", "El usuario de la mano debe ser mayor de 4 años.");
            }
        }
    }
}
