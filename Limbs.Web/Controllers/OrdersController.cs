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
using Limbs.Web.Repositories;
using Microsoft.AspNet.Identity;


namespace Limbs.Web.Controllers
{
    public class OrdersController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        public IOrdersRepository OrdersRepository { get; set; }

        private ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET: Orders/Index
        [Authorize]
        public ActionResult Index()
        {


            if(User.IsInRole("Usuario"))
            {
                return View("ListaUsuario");
            }
            else if(User.IsInRole("Embajador"))
            {

                var currentUserId = User.Identity.GetUserId();
                var orderList = OrdersRepository.GetByAssignedAmbassadorId(currentUserId);


                var pendingAssignationList = orderList.Where(o => o.Status == OrderStatus.PreAssigned).OrderByDescending(m => m.Id);
                var assignedList = orderList.Where(o => o.Status != OrderStatus.PreAssigned).OrderByDescending(m => m.Id);

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
        public ActionResult Accept(int Id)
        {


            var orderInDb = OrdersRepository.Get(Id);
            if (orderInDb == null) return HttpNotFound();

            // Comprobamos que el pedido esté asignado al usuario y esté pendiente de asignacion
            if(orderInDb.OrderUser.Id == User.Identity.GetUserId() && orderInDb.Status == OrderStatus.PreAssigned)
            {
                orderInDb.Status = OrderStatus.Pending;
                OrdersRepository.Update(orderInDb);
                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }
            
        }

        // GET: Orders/Reject/5
        [Authorize(Roles = "Embajador")]
        public ActionResult Reject(int Id)
        {
            var orderInDb = OrdersRepository.Get(Id);
            if (orderInDb == null) return HttpNotFound();

            // Comprobamos que el pedido esté asignado al usuario y esté pendiente de asignacion
            if (orderInDb.OrderUser.Id == User.Identity.GetUserId() && orderInDb.Status == OrderStatus.PreAssigned)
            {
                orderInDb.Status = OrderStatus.NotAssigned;
                orderInDb.OrderUser = null;
                OrdersRepository.Update(orderInDb);

                // (Seleccionar nuevo embajador)

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

            var orderID = id.Value;
            OrderModel orderModel = OrdersRepository.Get(orderID);


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


                    if (orderModel.Status == OrderStatus.PreAssigned || orderModel.Status == OrderStatus.Delivered)
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
            // "Order" solo tiene la ID consigo.
            OrderModel order = OrdersRepository.Get(Order.Id);

            if (order == null) return HttpNotFound();

            if (order.OrderUser.Id != User.Identity.GetUserId() || order.Status == OrderStatus.NotAssigned)
                return Content("AAs");


            if (order.Status == OrderStatus.Pending)
                order.Status = OrderStatus.Ready;
            else if (order.Status == OrderStatus.Ready)
                order.Status = OrderStatus.Delivered;

            // (actualizar DB)


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
                orderModel.Status = OrderStatus.Pending;
                OrdersRepository.Add(orderModel);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(orderModel);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            OrderModel orderModel;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var orderID = id.Value;

                orderModel = OrdersRepository.Get(orderID);
            }

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
                _context.Entry(orderModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(orderModel);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            OrderModel orderModel;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var orderId = id.Value;
                orderModel = OrdersRepository.Get(orderId);
            }
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
            OrderModel orderModel = OrdersRepository.Get(id);
            OrdersRepository.Remove(orderModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
