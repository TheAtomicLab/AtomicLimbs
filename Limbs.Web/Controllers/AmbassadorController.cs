using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Limbs.Web.Helpers;


namespace Limbs.Web.Controllers
{
    [DefaultAuthorize(Roles = AppRoles.Ambassador)]
    public class AmbassadorController : BaseController
    {
        // GET: /Ambassador/
        public ActionResult Index()
        {

            var currentUserId = User.Identity.GetUserId();

            IEnumerable<OrderModel> orderList = Db.OrderModels.Where(c => c.OrderAmbassador.UserId == currentUserId).Include(c => c.OrderRequestor).OrderByDescending(c => c.StatusLastUpdated).ToList();

            var deliveredOrdersCount = orderList.Count(c => c.Status == OrderStatus.Delivered);
            var pendingOrdersCount = orderList.Count(c => c.Status == OrderStatus.Pending || c.Status == OrderStatus.ArrangeDelivery || c.Status == OrderStatus.Ready);

            var pendingAssignationOrders = orderList.Where(o => o.Status == OrderStatus.PreAssigned).ToList();
            var pendingOrders = orderList.Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.ArrangeDelivery || o.Status == OrderStatus.Ready).ToList();
            var deliveredOrders = orderList.Where(o => o.Status == OrderStatus.Delivered).ToList();
            
            var viewModel = new ViewModels.AmbassadorPanelViewModel
            {
                PendingToAssignOrders = pendingAssignationOrders,
                PendingOrders = pendingOrders,
                DeliveredOrders = deliveredOrders,

                Stats = new ViewModels.OrderStats
                {
                    HandledOrders = deliveredOrdersCount,
                    PendingOrders = pendingOrdersCount
                }
            };


            return View(viewModel);
        }

        // GET: Ambassador/TermsAndConditions
        [OverrideAuthorize(Roles = AppRoles.Unassigned + "," + AppRoles.Ambassador)]
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        // GET: Ambassador/Create
        [OverrideAuthorize(Roles = AppRoles.Unassigned)]
        public ActionResult Create()
        {
            return View();
        }

        // GET: Ambassador/Edit
        [OverrideAuthorize(Roles = AppRoles.Ambassador + "," + AppRoles.Administrator)]
        public async Task<ActionResult> Edit(int? id)
        {
            AmbassadorModel ambassadorModel;
            var userId = User.Identity.GetUserId();
            if (!id.HasValue && User.IsInRole(AppRoles.Ambassador))
            {
                ambassadorModel = await Db.AmbassadorModels.FirstAsync(x => x.UserId == userId);
            }
            else
            {
                if (!id.HasValue) return HttpNotFound();
                ambassadorModel = await Db.AmbassadorModels.FindAsync(id);
            }
            if (ambassadorModel == null || !ambassadorModel.CanViewOrEdit(User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            ViewBag.ReturnUrl = Request.UrlReferrer?.AbsoluteUri;
            return View("Create", ambassadorModel);
        }

        // POST: Ambassador/Create
        [HttpPost]
        [OverrideAuthorize(Roles = AppRoles.Unassigned + "," + AppRoles.Ambassador + "," + AppRoles.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AmbassadorModel ambassadorModel, bool? termsAndConditions)
        {
            if (!User.IsInRole(AppRoles.Administrator))
            {
                ambassadorModel.Email = User.Identity.GetUserName();
                ambassadorModel.UserId = User.Identity.GetUserId();
            }

            await ValidateData(ambassadorModel, termsAndConditions);
            ViewBag.TermsAndConditions = termsAndConditions;
            if (!ModelState.IsValid) return View("Create", ambassadorModel);
            
            if (ambassadorModel.Id == 0 && (User.IsInRole(AppRoles.Unassigned) || User.IsInRole(AppRoles.Administrator)))
            {
                //CREATE
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Db));
                await userManager.RemoveFromRoleAsync(ambassadorModel.UserId, AppRoles.Unassigned);
                await userManager.AddToRoleAsync(ambassadorModel.UserId, AppRoles.Ambassador);

                Db.AmbassadorModels.Add(ambassadorModel);
                await Db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            if (!ambassadorModel.CanViewOrEdit(User)) return new HttpStatusCodeResult(HttpStatusCode.Conflict);

            //EDIT
            Db.Entry(ambassadorModel).State = EntityState.Modified;
            await Db.SaveChangesAsync();

            return RedirectToAction("Index", "Manage");
        }

        private async Task ValidateData(AmbassadorModel ambassadorModel, bool? termsAndConditions)
        {
            ModelState[nameof(ambassadorModel.Id)]?.Errors.Clear();
            ModelState[nameof(ambassadorModel.UserId)]?.Errors.Clear();
            ModelState[nameof(ambassadorModel.Email)]?.Errors.Clear();

            var pointAddress = ambassadorModel.Country + ", " + ambassadorModel.City + ", " + ambassadorModel.Address;
            ambassadorModel.Location = await Geolocalization.GetPointAsync(pointAddress);


            if (ambassadorModel.Birth > DateTime.UtcNow.AddYears(-AmbassadorModel.MinYear))
                ModelState.AddModelError(nameof(ambassadorModel.Birth), $@"Debes ser mayor de {AmbassadorModel.MinYear} años.");

            if (termsAndConditions.HasValue && !termsAndConditions.Value)
                ModelState.AddModelError(nameof(termsAndConditions), @"Debe aceptar terminos y condiciones.");
        }
    }
}

