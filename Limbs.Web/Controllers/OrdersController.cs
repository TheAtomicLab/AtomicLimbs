using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Limbs.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace Limbs.Web.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Orders/Index
        [Authorize]
        public async Task<ActionResult> Index()
        {

            if (User.IsInRole("Usuario"))
            {
                return View("ListaUsuario");
            }
            else if (User.IsInRole("Embajador"))
            {

                var currentUserId = User.Identity.GetUserId();
                var orderList = await db.OrderModels.Where(m => m.OrderUser.Id == currentUserId).ToListAsync();


                var pendingAssignationList = orderList.Where(o => o.Status == OrderStatus.NotAssigned).OrderByDescending(m => m.Id);
                var assignedList = orderList.Where(o => o.Status != OrderStatus.NotAssigned).OrderByDescending(m => m.Id);

                var viewModel = new ViewModels.EmbajadorOrderListViewModel()
                {
                    PendingToAssignOrders = pendingAssignationList,
                    AssignedOrders = assignedList
                };

                return View("ListaEmbajador", viewModel);
            }
            else // Admin?
            {
                return RedirectToAction("Index", "Home");
            }

        }


        [Authorize(Roles = "Embajador")]
        public ActionResult AcceptAssignation(int? Id)
        {
            var orderInDb = db.OrderModels.SingleOrDefault(m => m.Id == Id);
            if (orderInDb == null) return HttpNotFound();

            // Comprobamos que el pedido esté asignado al usuario y esté pendiente de asignacion
            if (orderInDb.OrderUser.Id == User.Identity.GetUserId() && orderInDb.Status == OrderStatus.NotAssigned)
            {
                orderInDb.Status = OrderStatus.Pending;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }

        }

        [Authorize(Roles = "Embajador")]
        public ActionResult RejectAssignation(int? Id)
        {
            var orderInDb = db.OrderModels.SingleOrDefault(m => m.Id == Id);
            if (orderInDb == null) return HttpNotFound();

            // Comprobamos que el pedido esté asignado al usuario y esté pendiente de asignacion
            if (orderInDb.OrderUser.Id == User.Identity.GetUserId() && orderInDb.Status == OrderStatus.NotAssigned)
            {
                // Rechazar.
                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }

        }




        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderModel orderModel = await db.OrderModels.FindAsync(id);
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View("Create1");
        }

        public ActionResult CreateHand()
        {
            return View("pedir_mano_inicio");
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [Authorize(Roles = "Usuario")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Design,Sizes,Comments")] OrderModel orderModel)
        {
            if (ModelState.IsValid)
            {

                // Asignar usuario que genero la orden (hay mejor manera pero es de prueba)
                var userId = User.Identity.GetUserId();
                orderModel.OrderRequestor = db.Users.SingleOrDefault(m => m.Id == userId);

                // ---- TEMPORAL --- Asignar al user embajador@limbs.com el manejo de la orden
                orderModel.OrderUser = db.Users.SingleOrDefault(m => m.Email == "embajador@limbs.com");


                orderModel.Status = OrderStatus.NotAssigned;
                db.OrderModels.Add(orderModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(orderModel);
        }

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderModel orderModel = await db.OrderModels.FindAsync(id);
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Comments,Status")] OrderModel orderModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(orderModel);
        }

        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderModel orderModel = await db.OrderModels.FindAsync(id);
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OrderModel orderModel = await db.OrderModels.FindAsync(id);
            db.OrderModels.Remove(orderModel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
