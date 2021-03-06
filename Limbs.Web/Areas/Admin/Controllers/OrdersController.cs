﻿using AutoMapper;
using Limbs.Web.Common.Extensions;
using Limbs.Web.Common.Mail;
using Limbs.Web.Common.Mail.Entities;
using Limbs.Web.Entities.Models;
using Limbs.Web.Entities.WebModels;
using Limbs.Web.Entities.WebModels.Admin.Models;
using Limbs.Web.Logic.Repositories.Interfaces;
using Limbs.Web.Logic.Services;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class OrdersController : AdminBaseController
    {
        private readonly string _fromEmail = ConfigurationManager.AppSettings["Mail.From"];

        private readonly IUserFiles _userFiles;
        private readonly IOrderNotificationService _ns;
        private readonly OrderService _os;

        public OrdersController(IUserFiles userFiles, IOrderNotificationService notificationService)
        {
            _userFiles = userFiles;
            _ns = notificationService;
            _os = new OrderService(Db);
        }
        
        // GET: Admin/Order
        public async Task<ActionResult> Index(OrderFilters filters)
        {
            var f = filters ?? new OrderFilters();
            var orderList = await _os.GetPaged(f);

            var model = new OrderListViewModel
            {
                List = Mapper.Map<IEnumerable<OrderAdminIndexViewModel>>(orderList),
                Filters = f,
            };

            return View(model);
        }

        // GET: Admin/Orders/CsvExport
        public async Task<FileContentResult> CsvExport()
        {
            var dataList = await Db.OrderModels.Include(c => c.OrderRequestor).Include(c => c.OrderAmbassador).OrderBy(o => o.Id).ToListAsync();
            var sb = new StringBuilder();

            var anyOrder = dataList.FirstOrDefault(x => x.OrderAmbassador != null);

            anyOrder = anyOrder ?? dataList.First();

            var orderTitles = anyOrder.GetTitles();
            var requestorTitles = anyOrder.OrderRequestor.GetTitles();
            var ambassadorTitles = anyOrder.OrderAmbassador?.GetTitles();

            var orderExportTitles = (ambassadorTitles != null) ? orderTitles.Union(requestorTitles.Union(ambassadorTitles)) : orderTitles.Union(requestorTitles);

            sb.AppendLine(string.Join(",", orderExportTitles));

            foreach (var order in dataList)
            {
                var orderText = order.ToString();
                sb.AppendLine(orderText);
            }

            DateTime dateTime = DateTime.Now;
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            DateTime dateTimeExport = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, timeZoneInfo);

            var dateTimeExportString = dateTimeExport.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var nameCsv = $"pedidos_{dateTimeExportString}.csv";

            var data = Encoding.UTF8.GetBytes(sb.ToString());
            var result = Encoding.UTF8.GetPreamble().Concat(data).ToArray();

            return File(result, "text/csv", nameCsv);
        }

        public async Task<ActionResult> LogCsvExport(int? orderId)
        {
            if (orderId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = await Db.OrderModels.Include(c => c.OrderRequestor).Include(c => c.OrderAmbassador).SingleAsync(x => x.Id == orderId);

            if (order == null)
            {
                return HttpNotFound();
            }

            var orderJson = JsonConvert.DeserializeObject<List<OrderLogItem>>(order.OrderLog);

            var sb = new StringBuilder();

            var titles = new List<string>
            {
                "Usuario",
                "Accion",
                "Fecha",
            };

            var orderTitles = order.GetTitles();
            var requestorTitles = order.OrderRequestor.GetTitles();
            var ambassadorTitles = order.OrderAmbassador?.GetTitles();

            var orderExportTitles = (ambassadorTitles != null) ? orderTitles.Union(requestorTitles.Union(ambassadorTitles)) : orderTitles.Union(requestorTitles);

            var exportTitles = titles.Union(orderExportTitles);

            sb.AppendLine(string.Join(",", exportTitles));

            foreach (var data in orderJson)
            {
                var dataList = new List<string>
                {
                    data.User,
                    string.Concat("\"",data.Message,"\""),
                    data.Date.ToString("G"),
                    data.OrderString,
                };

                var dataText = string.Join(",", dataList);
                sb.AppendLine(dataText);
            }

            var dateTimeExport = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            var nameCsv = $"order_{orderId}_{dateTimeExport}.csv";

            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", nameCsv);
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
        public async Task<ActionResult> Edit(OrderModel orderModel, HttpPostedFileBase orderPhoto, int selectPhoto)
        {
            if (!ModelState.IsValid)
            {
                return View(orderModel);
            }

            var isOk = await UpdateOrder(orderModel, orderPhoto, selectPhoto);

            if (!isOk)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index");
        }

        //WrongInfo
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> WrongInfo(WrongInfoModel model)
        {
            string subject = string.Empty;

            WrongInfoEmail modelEmail = new WrongInfoEmail
            {
                Comments = model.Comments,
                Fullname = model.Fullname_Requestor,
                Url = HttpContext.Request.UrlReferrer.ToString(),
                Url_Image = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~/Content/img/ej_mano_email.jpg")
            };

            MailMessage mailMessage = new MailMessage
            {
                From = _fromEmail,
                To = model.Email_Requestor
            };

            if (model.IsWrongImages)
            {
                if (!string.IsNullOrEmpty(model.Comments))
                {
                    subject = "[Acción requerida] Lamentablemente necesitamos más datos y otra foto para hacerte la prótesis";
                    modelEmail.Url_UserEdit = Url.Action("Edit", "Users", new { area = "" }, Request.Url.Scheme);

                    mailMessage.Body = CompiledTemplateEngine.Render("Mails.IncorrectPhotoAmbassadorExtraComment", modelEmail);
                }
                else
                {
                    subject = "[Acción requerida] Lamentablemente necesitamos otra foto para hacerte la prótesis";
                    mailMessage.Body = CompiledTemplateEngine.Render("Mails.IncorrectPhotoAmbassador", modelEmail);
                }
            }

            if (!string.IsNullOrEmpty(model.Comments) && !model.IsWrongImages)
            {
                subject = "[Acción requerida] Lamentablemente necesitamos más datos para hacerte la prótesis";
                mailMessage.Body = CompiledTemplateEngine.Render("Mails.IncorrectInfoComment", modelEmail);
            }

            mailMessage.Subject = subject;

            await AzureQueue.EnqueueAsync(mailMessage);
            if (model.Order_Id != null)
            {
                OrderModel order = await Db.OrderModels.FindAsync(model.Order_Id);
                if (order != null)
                {
                    order.LogMessage(User, "Request more info because of wrong info", order.ToString());
                    await Db.SaveChangesAsync();
                }
            }

            return RedirectToAction("Details", "Orders", new { id = model.Order_Id, area = "" });
        }

        public async Task<bool> UpdateOrder(OrderModel orderModel, HttpPostedFileBase file, int selectPhoto)
        {
            var oldOrder = await Db.OrderModels.Include(x => x.OrderRequestor).Include(x => x.OrderAmbassador).FirstOrDefaultAsync(x => x.Id == orderModel.Id);

            if (oldOrder == null)
            {
                return false;
            }

            if (!CanEditOrder(oldOrder))
            {
                return false;
            }

            string olderModelStr = oldOrder.ToString();

            //Campos a editar
            oldOrder.AmputationType = orderModel.AmputationType;
            oldOrder.ProductType = orderModel.ProductType;

            selectPhoto--;
            if (file != null)
            {
                UpdateOrderFile(oldOrder, file, selectPhoto);
            }
            //oldOrder.Sizes = orderModel.Sizes;
            oldOrder.Color = orderModel.Color;
            oldOrder.Comments = orderModel.Comments;
            oldOrder.StatusLastUpdated = DateTime.UtcNow;

            // TODO: ¿Send notification when changing status? 
            //var oldStatus = oldOrder.Status;
            //var newStatus = orderModel.Status;
            //
            //if (oldStatus != newStatus) oldOrder.Status = newStatus;
            //

            oldOrder.LogMessage(User, "Edited order", olderModelStr);
            Db.OrderModels.AddOrUpdate(oldOrder);
            //oldOrder.LogMessage(User, "Edited order ");

            await Db.SaveChangesAsync();

            return true;
        }

        private void UpdateOrderFile(OrderModel orderModel, HttpPostedFileBase file, int selectPhoto)
        {
            if (file == null || file.ContentLength == 0)
            {
                ModelState.AddModelError("nofile", @"Seleccione una foto.");
            }
            else
            {
                if (file.ContentLength > 1000000 * 5)
                {
                    ModelState.AddModelError("bigfile", @"La foto elegida es muy grande (max = 5 MB).");
                }
                if (!file.IsImage())
                {
                    ModelState.AddModelError("noimage", @"El archivo seleccionado no es una imagen.");
                }
            }

            var fileName = Guid.NewGuid().ToString("N") + ".jpg";
            var fileUrl = _userFiles.UploadOrderFile(file?.InputStream, fileName);

            var images = (orderModel.IdImage).Split(',');
            images[selectPhoto] = fileUrl.ToString();

            orderModel.IdImage = String.Join(",", images);

            //Db.OrderModels.AddOrUpdate(orderModel);
            //await Db.SaveChangesAsync();
        }

        // POST: Admin/Orders/EditStatus/5
        [HttpPost]
        [OverrideAuthorize(Roles = AppRoles.Ambassador)]
        public async Task<ActionResult> EditStatus(int orderId, OrderStatus newStatus, string returnUrl)
        {
            if (newStatus == OrderStatus.PreAssigned)
            {
                return new HttpUnauthorizedResult("use admin panel to assign ambassador");
            }

            var order = await Db.OrderModels.Include(x => x.OrderAmbassador).Include(x => x.OrderRequestor).FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                return HttpNotFound();
            }

            if (!CanEditOrder(order))
            {
                return HttpNotFound();
            }

            var oldStatus = order.Status;

            if (newStatus == OrderStatus.NotAssigned)
            {
                order.OrderAmbassador = null;
            }

            if (newStatus == OrderStatus.Rejected)
            {
                Db.OrderRefusedModels.Add(new OrderRefusedModels
                {
                    OrderId = order.Id,
                    AmbassadorId = order.OrderAmbassador.Id,
                    Date = DateTime.Now
                });
            }

            order.Status = newStatus;
            order.StatusLastUpdated = DateTime.UtcNow;
            order.LogMessage(User, $"Change status from {oldStatus} to {newStatus}");

            Db.OrderModels.AddOrUpdate(order);
            await Db.SaveChangesAsync();

            await _ns.SendStatusChangeNotification(order, oldStatus, newStatus);

            //Luego de actualizar el estado a rejected (SaveChanges)
            if (newStatus == OrderStatus.Rejected)
            {
                AmbassadorModel ambassador = await _os.GetAmbassadorAutoAssignAsync(order);

                if (ambassador != null)
                    await _os.AssignmentAmbassadorAsync(ambassador.Id, order.Id, User, _ns);
            }

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
            if (User.IsInRole(AppRoles.Administrator))
            {
                return true;
            }

            //check ownership
            if (order.OrderAmbassador != null)
            {
                return order.OrderAmbassador.UserId == User.Identity.GetUserId() ||
                       order.OrderRequestor.UserId == User.Identity.GetUserId();
            }

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
            {
                return HttpNotFound();
            }

            var orderRequestorLocation = order.OrderRequestor.Location;
            var ambassadorList = await Db.AmbassadorModels
                .OrderBy(x => x.Location.Distance(orderRequestorLocation))
                .ToListAsync();

            return View(new AssignOrderAmbassadorViewModel
            {
                Order = order,
                AmbassadorList = ambassadorList.Select(
                    ambassadorModel => new Tuple<AmbassadorModel, double, int>(
                        ambassadorModel,
                        ambassadorModel.Location.Distance(orderRequestorLocation) ?? 0,
                        Db.OrderModels.Count(o => o.OrderAmbassador.Id == ambassadorModel.Id)))
                    .ToList()
            });
        }

        public async Task<ActionResult> AssignAmbassadorAuto(int id)
        {
            OrderModel order = await Db.OrderModels.Include(p => p.OrderRequestor).FirstOrDefaultAsync(p => p.Id == id);
            if (order == null)
                return HttpNotFound();

            AmbassadorModel closestAmbassador = await _os.GetAmbassadorAutoAssignAsync(order);
            if (closestAmbassador == null)
            {
                TempData["msg"] = "No se han encontrado embajadores cercanos para asignar automáticamente";
                return RedirectToAction("Details", "Orders", new { order.Id, area = "" });
            }

            var result = await _os.AssignmentAmbassadorAsync(closestAmbassador.Id, id, User, _ns);
            if (!result)
                return HttpNotFound();

            return RedirectToAction("Index");
        }

        // GET: Admin/Orders/AssignAmbassador/5?idOrder=2
        public async Task<ActionResult> AssignAmbassador(int id, int idOrder)
        {
            var result = await _os.AssignmentAmbassadorAsync(id, idOrder,User, _ns);

            if (!result)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index");
        }


        // GET: Admin/Orders/SelectDelivery/?idOrder=2
        public async Task<ActionResult> SelectDelivery(int idOrder)
        {
            var order = await Db.OrderModels.Include(x => x.OrderRequestor).FirstOrDefaultAsync(x => x.Id == idOrder);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View("SelectDelivery", order);
        }

        // POST: Admin/Orders/AssignDelivery/?idOrder=2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignDelivery(HttpPostedFileBase file, OrderModel orderModel)
        {
            var order = await Db.OrderModels.Include(x => x.OrderAmbassador).Include(x => x.OrderRequestor).FirstAsync(x => x.Id == orderModel.Id);

            if (order == null)
            {
                return HttpNotFound();
            }

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