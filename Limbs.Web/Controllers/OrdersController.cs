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
    public class OrdersController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        public IOrdersRepository OrdersRepository { get; set; }

        private ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Orders
        public async Task<ActionResult> Index()
        {
            return View(OrdersRepository.Get());
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

                orderModel = OrdersRepository.Get(orderID);
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
