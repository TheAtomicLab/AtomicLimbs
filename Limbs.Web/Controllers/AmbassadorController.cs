using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;
using Limbs.Web.Models;
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
            var pendingOrdersCount = orderList.Count(c => c.Status == OrderStatus.Pending || c.Status == OrderStatus.Ready);

            var pendingAssignationOrders = orderList.Where(o => o.Status == OrderStatus.PreAssigned).ToList();
            var pendingOrders = orderList.Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Ready).ToList();
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
        [OverrideAuthorize(Roles = AppRoles.Unassigned)]
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

        // POST: Ambassador/Create
        [HttpPost]
        [OverrideAuthorize(Roles = AppRoles.Unassigned)]
        public async Task<ActionResult> Create(AmbassadorModel ambassadorModel)
        {
            if (!ModelState.IsValid) return View("Create", ambassadorModel);

            var pointAddress = ambassadorModel.Country + ", " + ambassadorModel.City + ", " + ambassadorModel.Address;
            
            ambassadorModel.Location = Geolocalization.GetPoint(pointAddress);
            ambassadorModel.Email = User.Identity.GetUserName();
            ambassadorModel.UserId = User.Identity.GetUserId();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Db));
            await userManager.RemoveFromRoleAsync(ambassadorModel.UserId, AppRoles.Unassigned);
            await userManager.AddToRoleAsync(ambassadorModel.UserId, AppRoles.Ambassador);

            Db.AmbassadorModels.Add(ambassadorModel);
            await Db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
