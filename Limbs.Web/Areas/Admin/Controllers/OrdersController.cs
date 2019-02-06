using Limbs.Web.Areas.Admin.Models;
using Limbs.Web.Common.Extensions;
using Limbs.Web.Entities.Models;
using Limbs.Web.Helpers;
using Limbs.Web.Models;
using Limbs.Web.Repositories.Interfaces;
using Limbs.Web.Services;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class OrdersController : AdminBaseController
    {
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
                List = orderList,
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
            if (model.IsWrongImages)
            {
                //send mail for wrong images
            }

            if (string.IsNullOrEmpty(model.Comments))
            {
                //send email for comments
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

            Db.OrderModels.AddOrUpdate(oldOrder);
            oldOrder.LogMessage(User, "Edited order", oldOrder.ToString());
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
            AmbassadorModel closestAmbassador = null;
            OrderModel order = await Db.OrderModels.Include(p => p.OrderRequestor).FirstOrDefaultAsync(p => p.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            DbGeography location = order.OrderRequestor.Location;
            closestAmbassador = await Db.AmbassadorModels.Include(p => p.User)
                                                    .FirstOrDefaultAsync(p => p.Location.Distance(location) <= 50d &&
                                                        p.User.EmailConfirmed &&
                                                        !p.OrderModel.Any(o => (o.Id == id && o.Status == OrderStatus.Rejected) || 
                                                            o.Status != OrderStatus.PreAssigned ||
                                                            o.Status != OrderStatus.Pending ||
                                                            o.Status != OrderStatus.Ready ||
                                                            o.Status != OrderStatus.ArrangeDelivery));

            if (closestAmbassador == null)
            {
                HttpResponseMessage response = await CallAndreaniApiAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return HttpNotFound();
                }

                string content = await response.Content.ReadAsStringAsync();
                XDocument createXml = XDocument.Parse(content);

                IEnumerable<UbicacionAndreani> ubicaciones = createXml.Descendants("item").Select(p => new UbicacionAndreani
                {
                    Descripcion = p.Element("Descripcion").Value,
                    Direccion = p.Element("Direccion").Value,
                    HoradeTrabajo = p.Element("HoradeTrabajo").Value,
                    Latitud = p.Element("Latitud").Value,
                    Longitud = p.Element("Longitud").Value,
                    Mail = p.Element("Mail").Value,
                    Numero = p.Element("Numero").Value,
                    Responsable = p.Element("Responsable").Value,
                    Resumen = p.Element("Resumen").Value,
                    Sucursal = p.Element("Sucursal").Value,
                    Telefono1 = p.Element("Telefono1").Value,
                    Telefono2 = p.Element("Telefono2").Value,
                    Telefono3 = p.Element("Telefono3").Value,
                    TipoSucursal = p.Element("TipoSucursal").Value,
                    TipoTelefono1 = p.Element("TipoTelefono1").Value,
                    TipoTelefono2 = p.Element("TipoTelefono2").Value,
                    TipoTelefono3 = p.Element("TipoTelefono3").Value,
                    Location = SiteHelper.GeneratePoint($"{p.Element("Latitud").Value},{p.Element("Longitud").Value}".Split(','))
                });
                var listAmbassadors = await Db.AmbassadorModels.ToListAsync();
                closestAmbassador = listAmbassadors.FirstOrDefault(p =>
                                                                    ubicaciones.Any(x =>
                                                                        x.Location.Distance(p.Location) <= 20d));
            }

            if (closestAmbassador == null)
            {
                TempData["msg"] = "No se han encontrado embajadores cercanos para asignar automáticamente";
                return RedirectToAction("Details", "Orders", new { id, area = "" });
            }

            HttpStatusCodeResult result = await AssignmentAmbassadorAsync(closestAmbassador.Id, id);
            if (result.StatusCode == 404)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index");
        }

        public async Task<HttpResponseMessage> CallAndreaniApiAsync()
        {
            string soapString = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                    <env:Envelope
                                        xmlns:env=""http://www.w3.org/2003/05/soap-envelope""
                                        xmlns:ns1=""urn:ConsultarSucursales""
                                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                                        xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                                        xmlns:ns2=""http://xml.apache.org/xml-soap""
                                        xmlns:ns3=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""
                                        xmlns:enc=""http://www.w3.org/2003/05/soap-encoding"">
                                        <env:Header>
                                            <ns3:Security env:mustUnderstand=""true"">
                                                <ns3:UsernameToken>
                                                    <ns3:Username></ns3:Username>
                                                    <ns3:Password></ns3:Password>
                                                </ns3:UsernameToken>
                                            </ns3:Security>
                                        </env:Header>
                                        <env:Body>
                                            <ns1:ConsultarSucursales env:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
                                                <Consulta xsi:type=""ns2:Map"">
                                                    <item>
                                                        <key xsi:type=""xsd:string"">consulta</key>
                                                        <value xsi:type=""ns2:Map"">
                                                            <item>
                                                                <key xsi:type=""xsd:string"">Localidad</key>
                                                                <value xsi:type=""xsd:string""></value>
                                                            </item>
                                                            <item>
                                                                <key xsi:type=""xsd:string"">CodigoPostal</key>
                                                                <value xsi:type=""xsd:string""></value>
                                                            </item>
                                                            <item>
                                                                <key xsi:type=""xsd:string"">Provincia</key>
                                                                <value xsi:type=""xsd:string""></value>
                                                            </item>
                                                        </value>
                                                    </item>
                                                </Consulta>
                                            </ns1:ConsultarSucursales>
                                        </env:Body>
                                    </env:Envelope>";

            HttpResponseMessage response = await SiteHelper.PostXmlRequestAsync("https://sucursales.andreani.com/ws", soapString, "ConsultarSucursales");
            return response;
        }

        // GET: Admin/Orders/AssignAmbassador/5?idOrder=2
        public async Task<ActionResult> AssignAmbassador(int id, int idOrder)
        {
            HttpStatusCodeResult result = await AssignmentAmbassadorAsync(id, idOrder);

            if (result.StatusCode == 404)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index");
        }

        private async Task<HttpStatusCodeResult> AssignmentAmbassadorAsync(int id, int idOrder)
        {
            OrderModel order = await Db.OrderModels.Include(x => x.OrderAmbassador).Include(x => x.OrderRequestor).FirstOrDefaultAsync(x => x.Id == idOrder);

            if (order == null)
            {
                return new HttpStatusCodeResult(404);
            }

            AmbassadorModel newAmbassador = await Db.AmbassadorModels.FindAsync(id);

            if (newAmbassador == null)
            {
                return new HttpStatusCodeResult(404);
            }

            AmbassadorModel oldAmbassador = order.OrderAmbassador;
            OrderStatus orderOldStatus = order.Status;

            order.OrderAmbassador = newAmbassador;
            order.Status = OrderStatus.PreAssigned;
            order.StatusLastUpdated = DateTime.UtcNow;
            order.LogMessage(User, $"Change ambassador from {(oldAmbassador != null ? oldAmbassador.Email : "no-data")} to {newAmbassador.Email}");

            await Db.SaveChangesAsync();
            await _ns.SendStatusChangeNotification(order, orderOldStatus, OrderStatus.PreAssigned);
            await _ns.SendAmbassadorChangedNotification(order, oldAmbassador, newAmbassador);

            return new HttpStatusCodeResult(200);
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