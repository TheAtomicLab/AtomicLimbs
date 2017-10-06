using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Limbs.Web.Areas.Admin.Models;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class ReportController : AdminBaseController
    {
        // GET: Admin/Report
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Panel", new {area = "Admin"});
        }

        /// <summary>
        /// los pedidos que están asignados y que pasaron más de x días sin imprimir
        /// </summary>
        /// <returns></returns>
        // GET: Admin/Report/DelayedOrders
        public ActionResult DelayedOrders(int daysBefore = 15)
        {
            var dateFrom = DateTime.UtcNow.AddDays(-daysBefore);
            var result = Db.OrderModels.Where(x =>
                x.Status == OrderStatus.Pending &&
                x.StatusLastUpdated <= dateFrom).Include(x => x.OrderRequestor).ToList();

            return View(new DelayedOrdersViewModel
            {
                DaysBefore = daysBefore,
                List = result,
            });
        }
    }
}