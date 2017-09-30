using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Limbs.Web.Models;
using Limbs.Web.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Web.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Security;


namespace Limbs.Web.Controllers
{
    [Authorize]
    public class AmbassadorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Lista embajadores
        // GET: Ambassador
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Index()
        {
            return View(await db.AmbassadorModels.ToListAsync());
        }

        // Para editar embajadores
        // GET: Ambassador/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmbassadorModel ambassadorModel = await db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }
            return View(ambassadorModel);
        }


        // GET: Ambassador/Create
        [Authorize(Roles = "Unassigned")]
        public ActionResult Create()
        {
            return View("Create");
        }


        private static IEnumerable<SelectListItem> GetCountryList()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(x => new SelectListItem
                {
                    Text = new RegionInfo(x.LCID).DisplayName,
                    Value = new RegionInfo(x.LCID).DisplayName,
                }).OrderBy(x => x.Value).DistinctBy(x => x.Value);
        }

        // POST: Ambassador/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Unassigned")]
        public async Task<ActionResult> Create(AmbassadorModel ambassadorModel)
        {
            ambassadorModel.Email = User.Identity.GetUserName();
            ambassadorModel.UserId = User.Identity.GetUserId();

            //ambassadorModel.OrderModelId = 0;
            if (ModelState.IsValid)
            {
                var pointAddress = ambassadorModel.Country + ", " + ambassadorModel.City + ", " + ambassadorModel.Address;

                var point = Helpers.Geolocalization.GetPointGoogle(pointAddress).Split(',');
                var lat = Convert.ToDouble(point[0].Replace('.', ','));
                var lng = Convert.ToDouble(point[1].Replace('.', ','));

                ambassadorModel.Lat = lat;
                ambassadorModel.Long = lng;

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                await userManager.RemoveFromRoleAsync(ambassadorModel.UserId, "Unassigned");
                await userManager.AddToRoleAsync(ambassadorModel.UserId, "Ambassador");

                db.AmbassadorModels.Add(ambassadorModel);
                await db.SaveChangesAsync();
                return RedirectToAction("AmbassadorPanel", "Ambassador");
            }

            return View("Create", ambassadorModel);
        }

        // GET: Ambassador/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmbassadorModel ambassadorModel = await db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }
            return View(ambassadorModel);
        }

        // POST: Ambassador/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // public async Task<ActionResult> Edit([Bind(Include = "Id,Name,LastName,Email,Phone,Birth,Gender,Country,City,Address,Lat,Long")] AmbassadorModel ambassadorModel)
        /*       --Leave comments for possible evolution-#idEvolution = 1#----##
                 
        public async Task<ActionResult> Edit([Bind(Include = "Id,AmbassadorName,Email,Birth,Gender,Address,Dni,AtributoEmbajador1,AtributoEmbajador2,AtributoEmbajador3")] AmbassadorModel ambassadorModel)
        */
        public async Task<ActionResult> Edit([Bind(Include = "Id,AmbassadorName,Email,Birth,Gender,Address,Dni")] AmbassadorModel ambassadorModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ambassadorModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ambassadorModel);
        }

        // GET: Ambassador/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmbassadorModel ambassadorModel = await db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }
            return View(ambassadorModel);
        }

        // POST: Ambassador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AmbassadorModel ambassadorModel = await db.AmbassadorModels.FindAsync(id);
            db.AmbassadorModels.Remove(ambassadorModel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        // GET: /AmbassadorPanel/
        [Authorize(Roles = "Ambassador")]
        public ActionResult AmbassadorPanel()
        {

            var currentUserId = User.Identity.GetUserId();
            AmbassadorModel ambassador = db.AmbassadorModels.Single(c => c.UserId == currentUserId);

            // Consulta DB. Cambiar con repos
            IEnumerable<OrderModel> orderList = db.OrderModels.Where(c => c.OrderAmbassador.UserId == currentUserId).Include(c => c.OrderRequestor).OrderByDescending(c => c.StatusLastUpdated).ToList();

            var DeliveredOrdersCount = orderList.Where(c => c.Status == OrderStatus.Delivered).Count();
            var PendingOrdersCount = orderList.Where(c => c.Status == OrderStatus.Pending || c.Status == OrderStatus.Ready).Count();

            var pendingAssignationOrders = orderList.Where(o => o.Status == OrderStatus.PreAssigned).ToList();
            var pendingOrders = orderList.Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Ready).ToList();
            var deliveredOrders = orderList.Where(o => o.Status == OrderStatus.Delivered).ToList();
            
            var lat = ambassador.Lat;    
            var lng = ambassador.Long;

            var pointIsValid = Helpers.Geolocalization.PointIsValid(lat,lng);

            var viewModel = new ViewModels.AmbassadorPanelViewModel()
            {
                PendingToAssignOrders = pendingAssignationOrders,
                PendingOrders = pendingOrders,
                DeliveredOrders = deliveredOrders,
                PointIsValid = pointIsValid,

                Stats = new ViewModels.OrderStats()
                {
                    HandledOrders = DeliveredOrdersCount,
                    PendingOrders = PendingOrdersCount
                }
            };


            return View(viewModel);
        }
        

        // GET: /OrderDetails/1
        [Authorize(Roles = "Ambassador")]
        public ActionResult OrderDetails(int? id)
        {
            var userId = User.Identity.GetUserId();
            OrderModel Order = db.OrderModels.Include(c => c.OrderRequestor).SingleOrDefault(c => c.Id == id && c.OrderAmbassador.UserId == userId);

            if (Order == null) return HttpNotFound();

            return View(Order);
        }


        // GET: Orders/Assignation/?id=5&action=accept
        // Aceptar o rechazar como embajador, pedidos pre-asignados.
        [Authorize(Roles = "Ambassador")]
        public ActionResult AssignOrder(int Id, string accion)
        {

            // Consulta DB. Cambiar con repos
            var orderInDb = db.OrderModels.Find(Id);

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
            else
                return HttpNotFound();

        }



        // GET: Orders/UpdateOrder
        // Cambiar de estado un pedido, marcar como "listo", o "entregado".
        [Authorize(Roles = "Ambassador")]
        public ActionResult UpdateOrder(int orderid, OrderStatus status)
        {
            // "Order" solo tiene la ID consigo.
            // Consulta DB. Cambiar con repos
            var orderInDB = db.OrderModels.Find(orderid);

            if (orderInDB == null) return HttpNotFound();

            if (orderInDB.OrderAmbassador.UserId != User.Identity.GetUserId() || orderInDB.Status == OrderStatus.NotAssigned)
                return HttpNotFound();


            if (status == OrderStatus.Ready && orderInDB.Status == OrderStatus.Pending)
            {
                orderInDB.Status = status;
                // Consulta DB. Cambiar con repos
                db.SaveChanges();

            }
            else if (status == OrderStatus.Delivered && orderInDB.Status == OrderStatus.Ready)
            {
                orderInDB.Status = status;
                orderInDB.StatusLastUpdated = DateTime.Now;
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
