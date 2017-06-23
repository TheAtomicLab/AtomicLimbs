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

            if(User.IsInRole("Usuario"))
            {
                return View("ListaUsuario");
            }
            else if(User.IsInRole("Embajador"))
            {

                var currentUserId = User.Identity.GetUserId();
                var orderList = await db.OrderModels.Include(m => m.OrderRequestor).Where(m => m.OrderUser.Id == currentUserId).ToListAsync();


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

        // GET: Orders/Accept/5
        [Authorize(Roles = "Embajador")]
        public ActionResult Accept(int? Id)
        {
            var orderInDb = db.OrderModels.SingleOrDefault(m => m.Id == Id);
            if (orderInDb == null) return HttpNotFound();

            // Comprobamos que el pedido esté asignado al usuario y esté pendiente de asignacion
            if(orderInDb.OrderUser.Id == User.Identity.GetUserId() && orderInDb.Status == OrderStatus.NotAssigned)
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

        // GET: Orders/Reject/5
        [Authorize(Roles = "Embajador")]
        public ActionResult Reject(int? Id)
        {
            var orderInDb = db.OrderModels.SingleOrDefault(m => m.Id == Id);
            if (orderInDb == null) return HttpNotFound();

            // Comprobamos que el pedido esté asignado al usuario y esté pendiente de asignacion
            if (orderInDb.OrderUser.Id == User.Identity.GetUserId() && orderInDb.Status == OrderStatus.NotAssigned)
            {

                orderInDb.OrderUser = null;
                db.SaveChanges();
                // Seleccionar nuevo embajador

                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }

        }


        // GET: Orders/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderModel orderModel = db.OrderModels.Include(m => m.OrderRequestor).SingleOrDefault(m => m.Id == id);
            if (orderModel == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Usuario"))
            {
                // mostrar contenido usuario
            }
            else if (User.IsInRole("Embajador"))
            {

                if (orderModel.OrderUser != null && orderModel.OrderUser.Id == User.Identity.GetUserId())
                {

                    var viewModel = new ViewModels.OrderDetailsEmbajadorViewModel()
                    {
                        Order = orderModel
                    };


                    if (orderModel.Status == OrderStatus.NotAssigned)
                        viewModel.CanChangeOrderStatus = false;
                    else
                        viewModel.CanChangeOrderStatus = true;


                    return View("DetallesEmbajador", viewModel);
                }
                else return HttpNotFound();

            }
            else
                return HttpNotFound();


            return Content("");
        }

        // POST: Orders/ChangeStatus
        [HttpPost]
        [Authorize(Roles = "Embajador")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatus(OrderModel Order)
        {
            var order = db.OrderModels.SingleOrDefault(m => m.Id == Order.Id);

            if (order == null) return HttpNotFound();

            if (order.OrderUser.Id != User.Identity.GetUserId() || order.Status == OrderStatus.NotAssigned)
                return Content("AAs");


            if (order.Status == OrderStatus.Pending)
                order.Status = OrderStatus.Ready;
            else if (order.Status == OrderStatus.Ready)
                order.Status = OrderStatus.Delivered;

            db.SaveChanges();


            return RedirectToAction("Details", new { id = Order.Id });


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
