using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Limbs.Web.Areas.Admin.Models;
using Limbs.Web.Entities.Models;
using Limbs.Web.Storage.Azure;
using Limbs.Web.Storage.Azure.TableStorage.Queries;

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

        /// <summary>
        /// errores por fecha
        /// </summary>
        /// <returns></returns>
        // GET: Admin/Report/Errors
        public ActionResult Errors(DateTime? date)
        {
            var dateFrom = date ?? DateTime.UtcNow;

            var result = new AppExceptionQuery(AzureStorageAccount.DefaultAccount).GetExceptions(dateFrom);
            
            return View(new AppExceptionViewModel
            {
                Date = dateFrom,
                List = result.ToList().Select(x => new AppExeptionItemViewModel
                {
                    DateTime = x.Date,
                    Message = x.Message,
                    StackTrace = x.StackTrace,
                    Url = x.Url,
                }).ToList(),
            });
        }
    }
}