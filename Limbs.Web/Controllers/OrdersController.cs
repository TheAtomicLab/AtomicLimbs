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
       ///se comentan los repository para el funcionamiento del front
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

      //  public IOrdersRepository OrdersRepository { get; set; }

       // private ApplicationDbContext _context;

   /*     public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }*/


        // GET: Orders/Index
        [Authorize]
        public ActionResult Index()
        {

            if(User.IsInRole("Usuario"))
            {
                return View("ListaUsuario");
            }
            else // Embajador
            {

                // Consulta DB. Cambiar con repos
                var currentUserId = User.Identity.GetUserId();
                IEnumerable<OrderModel> orderList = db.OrderModels.Where(c => c.OrderAmbassador.UserId == currentUserId).Include(c => c.OrderRequestor);


                var pendingAssignationList = orderList.Where(o => o.Status == OrderStatus.PreAssigned).OrderByDescending(m => m.Id);
                var assignedList = orderList.Where(o => o.Status != OrderStatus.PreAssigned).OrderByDescending(m => m.Id);

                var viewModel = new ViewModels.EmbajadorOrderListViewModel()
                {
                    PendingToAssignOrders = pendingAssignationList,
                    AssignedOrders = assignedList
                };

                return View("ListaEmbajador", viewModel);
            }

        }


        // GET: Orders/Assignation/?id=5&action=accept
        [Authorize(Roles = "Embajador")]
        public ActionResult Assignation(int Id, string accion)
        {

            // Consulta DB. Cambiar con repos
            var orderInDb = db.OrderModels.Find(Id);

            if (orderInDb == null) return HttpNotFound();

            // Comprobamos que el pedido esté asignado al embajador y esté pendiente de asignacion
            if (orderInDb.OrderAmbassador.UserId == User.Identity.GetUserId() && orderInDb.Status == OrderStatus.PreAssigned)
            {

                if(accion == "rechazar")
                {
                    orderInDb.Status = OrderStatus.NotAssigned;
                    orderInDb.OrderAmbassador = null;
                }
                else
                {
                    orderInDb.Status = OrderStatus.Pending;
                }

                // Consulta DB. Cambiar con repos
                db.SaveChanges();

                return RedirectToAction("Index");

            }
            else
                return HttpNotFound();

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

            // Consulta DB. Cambiar con repos
            var order = db.OrderModels.Include(c => c.OrderRequestor).Where(c => c.Id == orderID).SingleOrDefault();

            if (order == null)
            {
                return HttpNotFound();

            }

            if (User.IsInRole("Embajador"))
            {

                if (order.OrderAmbassador != null && order.OrderAmbassador.UserId == User.Identity.GetUserId())
                {

                    var viewModel = new ViewModels.OrderDetailsEmbajadorViewModel()
                    {
                        Order = order
                    };

                    if (order.Status == OrderStatus.PreAssigned || order.Status == OrderStatus.Delivered)
                        viewModel.CanChangeOrderStatus = false;
                    else
                        viewModel.CanChangeOrderStatus = true;

                    return View("DetallesEmbajador", viewModel);
                }
                else return HttpNotFound();

            }
            else
            {
                return Content("NO EMBAJADOR");
            }
     
            //return Content("");
        }



        // POST: Orders/Process
        [HttpPost]
        [Authorize(Roles = "Embajador")]
        [ValidateAntiForgeryToken]
        public ActionResult Process(OrderModel Order, OrderStatus Estado)
        {
            // "Order" solo tiene la ID consigo.
            // Consulta DB. Cambiar con repos
            var orderInDB = db.OrderModels.Find(Order.Id);

            if (orderInDB == null) return HttpNotFound();

            if (orderInDB.OrderAmbassador.UserId != User.Identity.GetUserId() || orderInDB.Status == OrderStatus.NotAssigned)
                return HttpNotFound();


            if(Estado == OrderStatus.Ready && orderInDB.Status == OrderStatus.Pending)
            {
                orderInDB.Status = OrderStatus.Ready;
                // Consulta DB. Cambiar con repos
                db.SaveChanges();
                
            }
            else if(Estado == OrderStatus.Delivered && orderInDB.Status == OrderStatus.Ready)
            {
                orderInDB.Status = OrderStatus.Delivered;
                // Consulta DB. Cambiar con repos
                db.SaveChanges();
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            return RedirectToAction("Details", new { id = Order.Id });


        }


        // GET: Orders/Create
        public ActionResult Create()
        {
            return View("Create");
        }

        public ActionResult CreateHand()
        {
            return View("pedir_mano_index");
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
                var currentUserId = User.Identity.GetUserId();
                // Consulta DB. Cambiar con repos
                var userModel = await db.UserModelsT.Where(c => c.UserId == currentUserId).SingleAsync();

                orderModel.OrderRequestor = userModel;
                orderModel.Status = OrderStatus.Pending;

                // Consulta DB. Cambiar con repos
                db.OrderModels.Add(orderModel);
                await db.SaveChangesAsync();


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

                // Consulta DB. Cambiar con repos
                orderModel = db.OrderModels.Find(orderID);

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

                //    _context.Entry(orderModel).State = EntityState.Modified;
                //    await _context.SaveChangesAsync();
                await db.SaveChangesAsync();
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

                // Consulta DB. Cambiar con repos
                orderModel = db.OrderModels.Find(orderId);

            }
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        public ActionResult pedir_mano_index()
        {
            return View();
        }

        public ActionResult pedir_brazo_medidas()
        {
            return View();
        }

        public ActionResult pedir_mano_medidas()
        {
            return View();
        }


        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //  OrderModel orderModel = OrdersRepository.Get(id);
            OrderModel ordelModel = await db.OrderModels.FindAsync(id);
            db.OrderModels.Remove(ordelModel);
            //OrdersRepository.Remove(orderModel);
            //    await _context.SaveChangesAsync();
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
