using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Limbs.Web.Entities.Models;
using Limbs.Web.ViewModels;
using Limbs.Web.Entities.WebModels.Admin.Models;
using Limbs.Web.Storage.Azure;
using Limbs.Web.Storage.Azure.TableStorage.Queries;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class ReportController : AdminBaseController
    {
        // GET: Admin/Report
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Panel", new { area = "Admin" });
        }

        /// <summary>
        /// los pedidos que están asignados y que pasaron más de x días sin imprimir
        /// </summary>
        /// <returns></returns>
        // GET: Admin/Report/DelayedOrders
        public async Task<ActionResult> DelayedOrders(int daysBefore = 15)
        {
            var dateFrom = DateTime.UtcNow.AddDays(-daysBefore);
            var result = await Db.OrderModels.Where(x =>
                x.Status == OrderStatus.Pending &&
                x.StatusLastUpdated <= dateFrom)
                    .Include(x => x.OrderRequestor)
                    .Include(p => p.OrderAmbassador)
                .ToListAsync();

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
                    CustomMessage = x.CustomMessage,
                }).ToList(),
            });
        }

        [HttpGet]
        public ActionResult InfoCOVID()
        {
            var viewModel = new COVIDInfoAdminViewModel();

            var orderList = Db.CovidOrganizationModels
                            .OrderByDescending(t=>t.Quantity)
                            .ToList();

            viewModel.TotalOrders = orderList.Count();
            viewModel.TotalQuantity = orderList.Sum(t => t.Quantity);
            viewModel.RankingOrders = orderList.Take(10).ToList();

            var assignmentList = Db.CovidOrgAmbassadorModels
                                .Include(t => t.CovidAmbassador)
                                .Include(t => t.CovidAmbassador.Ambassador)
                                .Include(t => t.CovidOrg)
                                .ToList();

            viewModel.AssignmentList = assignmentList;
            viewModel.TotalAssignedAmbassadors = assignmentList.Select(t => t.CovidAmbassador).Distinct().Count();
            viewModel.TotalAssignedQuantity = assignmentList.Sum(t => t.Quantity);

            return View(viewModel);
        }
    }
}