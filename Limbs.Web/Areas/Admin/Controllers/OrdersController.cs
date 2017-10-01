﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Areas.Admin.Models;
using Limbs.Web.Models;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class OrdersController : AdminBaseController
    {
        // GET: Admin/Order
        public ActionResult Index()
        {
            //TODO (ale): implementar paginacion, export to excel, ordenamiento
            var orderList = Db.OrderModels.Include(c => c.OrderRequestor).Include(c => c.OrderAmbassador).OrderBy(x => x.Date).ToList();
            return View(orderList);
        }

        // GET: Admin/Orders/Details/5
        public ActionResult Details(int id)
        {
            //Mismos detalles para usuarios y admin
            return RedirectToAction("Details", "Orders", new { id, area = "" });
        }

        // GET: Admin/Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Consulta DB. Cambiar con repos
            var orderModel = Db.OrderModels.Find(id.Value);

            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // POST: Admin/Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OrderModel orderModel)
        {
            if (!ModelState.IsValid) return View(orderModel);
            
            //TODO (ale): implement

            await Db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Admin/Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderModel = Db.OrderModels.Find(id.Value);
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            
            return View(orderModel);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var orderModel = await Db.OrderModels.FindAsync(id);

            if (orderModel == null)
            {
                return HttpNotFound();
            }
            
            Db.OrderModels.Remove(orderModel);
            await Db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        
        // GET: Admin/Orders/SelectAmbassador/5
        public ActionResult SelectAmbassador(int idOrder)
        {
            var order = Db.OrderModels.Find(idOrder);

            if (order == null)
                return HttpNotFound();

            //TODO (ale): ordenar por distancia de la orden
            IEnumerable<AmbassadorModel> ambassadorList = Db.AmbassadorModels.ToList();

            return View(new AssignOrderAmbassadorViewModel
            {
                Order = order,
                AmbassadorList = ambassadorList,
            });

        }

        // GET: Admin/Orders/AssignAmbassador/5?idOrder=2
        public ActionResult AssignAmbassador(int id, int idOrder)
        {
            var order = Db.OrderModels.Find(idOrder);

            if (order == null)
                return HttpNotFound();

            var ambassador = Db.AmbassadorModels.Find(id);

            if (ambassador == null)
                return HttpNotFound();

            order.OrderAmbassador = ambassador;
            order.Status = OrderStatus.PreAssigned;
            order.StatusLastUpdated = DateTime.UtcNow;
            Db.SaveChanges();

            //TODO (ale): notificar a los usuarios el cambio de estado + asignacion

            return RedirectToAction("Index");
        }
    }
}