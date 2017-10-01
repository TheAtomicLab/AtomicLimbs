using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Limbs.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Limbs.Web.Helpers;


namespace Limbs.Web.Controllers
{
    [Authorize]
    public class AmbassadorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ambassador/Create
        [Authorize(Roles = "Unassigned")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ambassador/Create
        [HttpPost]
        [Authorize(Roles = "Unassigned")]
        public async Task<ActionResult> Create(AmbassadorModel ambassadorModel)
        {
            if (!ModelState.IsValid) return View("Create", ambassadorModel);

            var pointAddress = ambassadorModel.Country + ", " + ambassadorModel.City + ", " + ambassadorModel.Address;

            //TODO (ale): refactor
            var point = Geolocalization.GetPointGoogle(pointAddress).Split(',');
            var lat = Convert.ToDouble(point[0].Replace('.', ','));
            var lng = Convert.ToDouble(point[1].Replace('.', ','));

            ambassadorModel.Location = Geolocalization.GeneratePoint(lat, lng);
            
            ambassadorModel.Email = User.Identity.GetUserName();
            ambassadorModel.UserId = User.Identity.GetUserId();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            await userManager.RemoveFromRoleAsync(ambassadorModel.UserId, "Unassigned");
            await userManager.AddToRoleAsync(ambassadorModel.UserId, "Ambassador");

            db.AmbassadorModels.Add(ambassadorModel);
            await db.SaveChangesAsync();
            return RedirectToAction("AmbassadorPanel", "Ambassador");
        }
        
        // GET: /AmbassadorPanel/
        [Authorize(Roles = "Ambassador")]
        public ActionResult AmbassadorPanel()
        {

            var currentUserId = User.Identity.GetUserId();
            AmbassadorModel ambassador = db.AmbassadorModels.Single(c => c.UserId == currentUserId);

            // Consulta DB. Cambiar con repos
            IEnumerable<OrderModel> orderList = db.OrderModels.Where(c => c.OrderAmbassador.UserId == currentUserId).Include(c => c.OrderRequestor).OrderByDescending(c => c.StatusLastUpdated).ToList();

            var deliveredOrdersCount = orderList.Count(c => c.Status == OrderStatus.Delivered);
            var pendingOrdersCount = orderList.Count(c => c.Status == OrderStatus.Pending || c.Status == OrderStatus.Ready);

            var pendingAssignationOrders = orderList.Where(o => o.Status == OrderStatus.PreAssigned).ToList();
            var pendingOrders = orderList.Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Ready).ToList();
            var deliveredOrders = orderList.Where(o => o.Status == OrderStatus.Delivered).ToList();
            
            var lat = ambassador.Location.Latitude;    
            var lng = ambassador.Location.Longitude;

            var pointIsValid = Geolocalization.PointIsValid(lat,lng);

            var viewModel = new ViewModels.AmbassadorPanelViewModel
            {
                PendingToAssignOrders = pendingAssignationOrders,
                PendingOrders = pendingOrders,
                DeliveredOrders = deliveredOrders,
                PointIsValid = pointIsValid,

                Stats = new ViewModels.OrderStats
                {
                    HandledOrders = deliveredOrdersCount,
                    PendingOrders = pendingOrdersCount
                }
            };


            return View(viewModel);
        }
        

        // GET: /OrderDetails/1
        [Authorize(Roles = "Ambassador")]
        public ActionResult OrderDetails(int? id)
        {
            var userId = User.Identity.GetUserId();
            var order = db.OrderModels.Include(c => c.OrderRequestor).SingleOrDefault(c => c.Id == id && c.OrderAmbassador.UserId == userId);

            if (order == null) return HttpNotFound();

            return View(order);
        }


        // GET: Orders/Assignation/?id=5&action=accept
        // Aceptar o rechazar como embajador, pedidos pre-asignados.
        [Authorize(Roles = "Ambassador")]
        public ActionResult AssignOrder(int id, string accion)
        {

            // Consulta DB. Cambiar con repos
            var orderInDb = db.OrderModels.Find(id);

            if (orderInDb == null) return HttpNotFound();

            // Comprobamos que el pedido esté asignado al embajador y esté pendiente de asignacion
            if (orderInDb.OrderAmbassador.UserId == User.Identity.GetUserId() && orderInDb.Status == OrderStatus.PreAssigned)
            {

                if (accion == "rechazar")
                {
                    orderInDb.Status = OrderStatus.NotAssigned;
                    orderInDb.OrderAmbassador = null;
                }
                else
                {
                    orderInDb.Status = OrderStatus.Pending;
                }
                orderInDb.StatusLastUpdated = DateTime.Now;

                // Consulta DB. Cambiar con repos
                db.SaveChanges();

                return RedirectToAction("AmbassadorPanel");

            }
            return HttpNotFound();
        }



        // GET: Orders/UpdateOrder
        // Cambiar de estado un pedido, marcar como "listo", o "entregado".
        [Authorize(Roles = "Ambassador")]
        public ActionResult UpdateOrder(int orderid, OrderStatus status)
        {
            // "Order" solo tiene la ID consigo.
            // Consulta DB. Cambiar con repos
            var orderInDb = db.OrderModels.Find(orderid);

            if (orderInDb == null) return HttpNotFound();

            if (orderInDb.OrderAmbassador.UserId != User.Identity.GetUserId() || orderInDb.Status == OrderStatus.NotAssigned)
                return HttpNotFound();


            if (status == OrderStatus.Ready && orderInDb.Status == OrderStatus.Pending)
            {
                orderInDb.Status = status;
                // Consulta DB. Cambiar con repos
                db.SaveChanges();

            }
            else if (status == OrderStatus.Delivered && orderInDb.Status == OrderStatus.Ready)
            {
                orderInDb.Status = status;
                orderInDb.StatusLastUpdated = DateTime.Now;
                // Consulta DB. Cambiar con repos
                db.SaveChanges();
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return RedirectToAction("AmbassadorPanel");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
