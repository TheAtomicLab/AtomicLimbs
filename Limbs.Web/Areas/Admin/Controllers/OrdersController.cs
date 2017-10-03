using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Limbs.Web.Areas.Admin.Models;
using Limbs.Web.Models;
using Limbs.Web.Repositories.Interfaces;
using Microsoft.AspNet.Identity;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class OrdersController : AdminBaseController
    {
        private readonly IUserFiles _userFiles;

        public OrdersController(IUserFiles userFiles)
        {
            _userFiles = userFiles;
        }

        // GET: Admin/Order
        public ActionResult Index()
        {
            //TODO (ale): implementar paginacion, export to excel, ordenamiento
            var orderList = Db.OrderModels.Include(c => c.OrderRequestor).Include(c => c.OrderAmbassador).OrderByDescending(x => x.Date).ToList();
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
            //TODO (ale): implementar segun los campos que tengan sentido editarse
            throw new NotImplementedException();

            if (!ModelState.IsValid) return View(orderModel);

            orderModel.LogMessage(User, "Edited order");
            Db.OrderModels.AddOrUpdate(orderModel);
            await Db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: Admin/Orders/EditStatus/5
        [HttpPost]
        [OverrideAuthorize(Roles = AppRoles.Ambassador)]
        public async Task<ActionResult> EditStatus(int orderId, OrderStatus newStatus, string returnUrl)
        {
            if (newStatus == OrderStatus.PreAssigned) return new HttpUnauthorizedResult("use admin panel to assign ambassador");

            var order = Db.OrderModels.Find(orderId);

            if (order == null) return HttpNotFound();

            if (!CanEditOrder(order, newStatus)) return HttpNotFound();
            
            if (newStatus == OrderStatus.NotAssigned)
                order.OrderAmbassador = null;
            order.LogMessage(User, $"Change status from {order.Status} to {newStatus}");
            order.Status = newStatus;
            order.StatusLastUpdated = DateTime.UtcNow;
            Db.OrderModels.AddOrUpdate(order);
            await Db.SaveChangesAsync();

            //TODO (ale): notificar a los usuarios el cambio de estado + asignacion

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

        private bool CanEditOrder(OrderModel order, OrderStatus newStatus)
        {
            if (User.IsInRole(AppRoles.Administrator)) return true;

            //check ownership
            if (order.OrderAmbassador != null)
                return order.OrderAmbassador.UserId == User.Identity.GetUserId() ||
                       order.OrderRequestor.UserId == User.Identity.GetUserId();
            return order.OrderRequestor.UserId == User.Identity.GetUserId();
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
            var order = Db.OrderModels.Include(x => x.OrderRequestor).FirstOrDefault(x => x.Id == idOrder);

            if (order == null)
                return HttpNotFound();

            var orderRequestorLocation = order.OrderRequestor.Location;
            var ambassadorList = Db.AmbassadorModels
                .OrderBy(x => x.Location.Distance(orderRequestorLocation))
                .ToList()
                .Select(ambassadorModel => new Tuple<AmbassadorModel, double>(
                    ambassadorModel, 
                    ambassadorModel.Location.Distance(orderRequestorLocation) ?? 0))
                    .ToList();

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

            order.LogMessage(User, $"Change ambassador from {(order.OrderAmbassador != null ? order.OrderAmbassador.Email : "no-data")} to {ambassador.Email}");
            order.OrderAmbassador = ambassador;
            order.Status = OrderStatus.PreAssigned;
            order.StatusLastUpdated = DateTime.UtcNow;
            Db.SaveChanges();

            //TODO (ale): notificar a los usuarios el cambio de estado + asignacion

            return RedirectToAction("Index");
        }

        // GET: Admin/Orders/SelectDelivery/?idOrder=2
        public ActionResult SelectDelivery(int idOrder)
        {
            var order = Db.OrderModels.Include(x => x.OrderRequestor).FirstOrDefault(x => x.Id == idOrder);

            if (order == null)
                return HttpNotFound();

            return View("SelectDelivery", order);
        }

        // POST: Admin/Orders/AssignDelivery/?idOrder=2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignDelivery(HttpPostedFileBase file, OrderModel orderModel)
        {
            var order = Db.OrderModels.Find(orderModel.Id);

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
            Db.SaveChanges();
            //TODO (ale): notificar a los usuarios el cambio de estado + asignacion

            return RedirectToAction("Index");
        }
    }
}