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
        public ActionResult Index()
        {

            return View();

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
        public async Task<ActionResult> Create([Bind(Include = "Id,Design,Sizes,Comments")] OrderModel orderModel)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity.GetUserId();
                // Consulta DB. Cambiar con repos
                var userModel = await db.UserModelsT.Where(c => c.UserId == currentUserId).SingleAsync();
                orderModel.OrderRequestor = userModel;

                // <TO DO: ASIGNAR EMBAJADOR ACA>
                orderModel.Status = OrderStatus.PreAssigned;
                orderModel.StatusLastUpdated = DateTime.Now;
                orderModel.Date = DateTime.Now;
                
                

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
