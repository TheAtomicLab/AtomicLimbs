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
        }

    */

        // GET: Orders
        public async Task<ActionResult> Index()
        {
            return View(await db.OrderModels.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            OrderModel orderModel;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var orderID = id.Value;

              //  orderModel = OrdersRepository.Get(orderID);
              orderModel = await db.OrderModels.FindAsync(id);
            }

            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
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
                orderModel.Status = OrderStatus.Pending;
                //OrdersRepository.Add(orderModel);
                db.OrderModels.Add(orderModel);
                await db.SaveChangesAsync();
                //  await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(orderModel);
        }

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            OrderModel orderModel;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var orderID = id.Value;

               // orderModel = OrdersRepository.Get(orderID);
               orderModel = await db.OrderModels.FindAsync(id);
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
        public async Task<ActionResult> Delete(int? id)
        {
            OrderModel orderModel;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var orderId = id.Value;
              //  orderModel = OrdersRepository.Get(orderId);
              orderModel = await db.OrderModels.FindAsync(id);
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
                //       _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
