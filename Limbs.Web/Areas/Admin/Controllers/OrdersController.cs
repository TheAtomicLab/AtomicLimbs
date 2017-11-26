using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Limbs.Web.Areas.Admin.Models;
using Limbs.Web.Entities.Models;
using Limbs.Web.Repositories.Interfaces;
using Limbs.Web.Services;
using Microsoft.AspNet.Identity;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class OrdersController : AdminBaseController
    {
        private readonly IUserFiles _userFiles;
        private readonly IOrderNotificationService _ns;

        public OrdersController(IUserFiles userFiles, IOrderNotificationService notificationService)
        {
            _userFiles = userFiles;
            _ns = notificationService;
        }

        // GET: Admin/Order
        public async Task<ActionResult> Index()
        {
            //TODO (ale): implementar paginacion, export to excel, ordenamiento
            var orderList = await Db.OrderModels.Include(c => c.OrderRequestor).Include(c => c.OrderAmbassador).OrderByDescending(x => x.Date).ToListAsync();
            return View(orderList);
        }

        // GET: Admin/Orders/Details/5
        public ActionResult Details(int id)
        {
            //Mismos detalles para usuarios y admin
            return RedirectToAction("Details", "Orders", new { id, area = "" });
        }

        // GET: Admin/Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Consulta DB. Cambiar con repos
            var orderModel = await Db.OrderModels.FindAsync(id.Value);

            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // POST: Admin/Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderModel orderModel)
        {
            //TODO (ale): implementar segun los campos que tengan sentido editarse
            throw new NotImplementedException();

            //if (!ModelState.IsValid) return View(orderModel);
            //
            //orderModel.LogMessage(User, "Edited order");
            //Db.OrderModels.AddOrUpdate(orderModel);
            //await Db.SaveChangesAsync();
            //
            //return RedirectToAction("Index");
        }

        // POST: Admin/Orders/EditStatus/5
        [HttpPost]
        [OverrideAuthorize(Roles = AppRoles.Ambassador)]
        public async Task<ActionResult> EditStatus(int orderId, OrderStatus newStatus, string returnUrl)
        {
            if (newStatus == OrderStatus.PreAssigned) return new HttpUnauthorizedResult("use admin panel to assign ambassador");

            var order = await Db.OrderModels.Include(x => x.OrderAmbassador).Include(x => x.OrderRequestor).FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null) return HttpNotFound();

            if (!CanEditOrder(order)) return HttpNotFound();

            var oldStatus = order.Status;

            if (newStatus == OrderStatus.NotAssigned) order.OrderAmbassador = null;
            order.Status = newStatus;
            order.StatusLastUpdated = DateTime.UtcNow;
            order.LogMessage(User, $"Change status from {oldStatus} to {newStatus}");
            
            Db.OrderModels.AddOrUpdate(order);
            await Db.SaveChangesAsync();

            await _ns.SendStatusChangeNotification(order, oldStatus, newStatus);

            return RedirectToLocal(returnUrl);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) && Request.UrlReferrer != null && Url.IsLocalUrl(Request.UrlReferrer.PathAndQuery))
            {
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return User.IsInRole(AppRoles.Administrator) ? 
                RedirectToAction("Index", "Home") : 
                RedirectToAction("RedirectUser", "Account");
        }

        private bool CanEditOrder(OrderModel order)
        {
            if (User.IsInRole(AppRoles.Administrator)) return true;

            //check ownership
            if (order.OrderAmbassador != null)
                return order.OrderAmbassador.UserId == User.Identity.GetUserId() ||
                       order.OrderRequestor.UserId == User.Identity.GetUserId();
            return order.OrderRequestor.UserId == User.Identity.GetUserId();
        }

        // GET: Admin/Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderModel = await Db.OrderModels.FindAsync(id.Value);
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
        public async Task<ActionResult> SelectAmbassador(int idOrder)
        {
            var order = await Db.OrderModels.Include(x => x.OrderRequestor).FirstOrDefaultAsync(x => x.Id == idOrder);

            if (order == null)
                return HttpNotFound();

            var orderRequestorLocation = order.OrderRequestor.Location;
            var ambassadorList = await Db.AmbassadorModels
                .OrderBy(x => x.Location.Distance(orderRequestorLocation))
                .ToListAsync();

            return View(new AssignOrderAmbassadorViewModel
            {
                Order = order,
                AmbassadorList = ambassadorList.Select(
                    ambassadorModel => new Tuple<AmbassadorModel, double>(
                        ambassadorModel,
                        ambassadorModel.Location.Distance(orderRequestorLocation) ?? 0))
                    .ToList(),
            });

        }

        // GET: Admin/Orders/AssignAmbassador/5?idOrder=2
        public async Task<ActionResult> AssignAmbassador(int id, int idOrder)
        {
            var order = await Db.OrderModels.Include(x => x.OrderAmbassador).Include(x => x.OrderRequestor).FirstOrDefaultAsync(x => x.Id == idOrder);

            if (order == null)
                return HttpNotFound();

            var newAmbassador = await Db.AmbassadorModels.FindAsync(id);

            if (newAmbassador == null)
                return HttpNotFound();

            var oldAmbassador = order.OrderAmbassador;
            var orderOldStatus = order.Status;

            order.OrderAmbassador = newAmbassador;
            order.Status = OrderStatus.PreAssigned;
            order.StatusLastUpdated = DateTime.UtcNow;
            order.LogMessage(User, $"Change ambassador from {(oldAmbassador != null ? oldAmbassador.Email : "no-data")} to {newAmbassador.Email}");

            await Db.SaveChangesAsync();
            await _ns.SendStatusChangeNotification(order, orderOldStatus, OrderStatus.PreAssigned);
            await _ns.SendAmbassadorChangedNotification(order, oldAmbassador, newAmbassador);

            return RedirectToAction("Index");
        }

        // GET: Admin/Orders/SelectDelivery/?idOrder=2
        public async Task<ActionResult> SelectDelivery(int idOrder)
        {
            var order = await Db.OrderModels.Include(x => x.OrderRequestor).FirstOrDefaultAsync(x => x.Id == idOrder);

            if (order == null)
                return HttpNotFound();

            return View("SelectDelivery", order);
        }

        // POST: Admin/Orders/AssignDelivery/?idOrder=2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignDelivery(HttpPostedFileBase file, OrderModel orderModel)
        {
            var order = await Db.OrderModels.Include(x => x.OrderAmbassador).Include(x => x.OrderRequestor).FirstAsync(x => x.Id == orderModel.Id);

            if (order == null)
                return HttpNotFound();

            if (file != null)
            {
                var fileName = Guid.NewGuid().ToString("N") + file.FileName;
                var fileUrl = _userFiles.UploadOrderFile(file.InputStream, fileName);

                order.DeliveryPostalLabel = fileUrl.AbsoluteUri;
                order.LogMessage(User, $"New delivery postal label at {fileUrl.AbsoluteUri}");
            }

            order.LogMessage(User, $"Change delivery from {order.DeliveryCourier} to {orderModel.DeliveryCourier}");
            order.DeliveryCourier = orderModel.DeliveryCourier;
            order.DeliveryTrackingCode = orderModel.DeliveryTrackingCode;
            order.Status = OrderStatus.Ready;

            await Db.SaveChangesAsync();
            await _ns.SendDeliveryInformationNotification(order);

            return RedirectToAction("Index");
        }
    }
}