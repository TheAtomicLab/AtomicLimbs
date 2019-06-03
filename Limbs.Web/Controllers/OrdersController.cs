using Limbs.Web.Common.Extensions;
using Limbs.Web.Entities.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using Limbs.Web.Logic.Repositories.Interfaces;
using Limbs.Web.Logic.Services;
using Limbs.Web.ViewModels;
using AutoMapper;
using Newtonsoft.Json;

namespace Limbs.Web.Controllers
{
    [DefaultAuthorize(Roles = AppRoles.Requester)]
    public class OrdersController : BaseController
    {
        private readonly IUserFiles _userFiles;
        private readonly IOrderNotificationService _ns;

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public OrdersController(IUserFiles userFiles, IOrderNotificationService notificationService)
        {
            _userFiles = userFiles;
            _ns = notificationService;
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var orderModel = await Db.OrderModels.FindAsync(id);
            if (orderModel == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var orderUpdateModel = Mapper.Map<OrderUpdateModel>(orderModel);
            orderUpdateModel.PreviousAmputationTypeId = orderModel.AmputationTypeFkId ?? 0;

            orderUpdateModel.HasDesign = await Db.RenderModels.AnyAsync(p => p.AmputationTypeId == orderModel.AmputationTypeFkId);

            var listAmputations = await Db.AmputationTypeModels.ToListAsync();
            var listAmputationDesign = new List<AmputationDesign>();

            foreach (var amputation in listAmputations)
            {
                var hasDesign = await Db.RenderModels.AnyAsync(p => p.AmputationTypeId == amputation.Id);

                listAmputationDesign.Add(new AmputationDesign
                {
                    Amputation = amputation,
                    HasDesign = hasDesign
                });
            }

            ViewData["ListAmputations"] = listAmputationDesign;
            ViewData["renderColors"] = await Db.ColorModels.Where(p => p.AmputationTypeId == orderModel.AmputationTypeFkId).ToListAsync();

            return View(orderUpdateModel);
        }

        [HttpGet]
        public async Task<ActionResult> GetColors(int? amputationId)
        {
            bool isSuccessfully = false;
            if (amputationId == null) return Json(new { isSuccessfully });

            var colors = await Db.ColorModels.Where(p => p.AmputationTypeId == amputationId).ToListAsync();

            isSuccessfully = true;

            return Json(new
            {
                isSuccessfully,
                colors
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OrderUpdateModel orderUpdateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(orderUpdateModel);
            }

            OrderModel orderModel = await Db.OrderModels.Include(p => p.RenderPieces).FirstOrDefaultAsync(x => x.Id == orderUpdateModel.Id);
            if (orderModel == null) new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            string olderModelStr = orderModel.ToString();
            orderModel = Mapper.Map(orderUpdateModel, orderModel);

            if (Request.Files != null && Request.Files.Count > 0)
            {
                foreach (string fileStr in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileStr];

                    if (file.ContentLength == 0 || file.ContentLength > 1000000 * 5 || !file.IsImage())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ocurrio un error en la imagen");

                    var fileName = Guid.NewGuid().ToString("N") + ".jpg";
                    var fileUrl = _userFiles.UploadOrderFile(file.InputStream, fileName);

                    orderModel.IdImage += $",{fileUrl.ToString()}";
                }
            }

            if (orderUpdateModel.PreviousAmputationTypeId != orderUpdateModel.AmputationTypeFkId)
            {
                if (orderModel.RenderPieces != null && orderModel.RenderPieces.Any())
                    Db.OrderRenderPieceModels.RemoveRange(orderModel.RenderPieces);

                if (orderUpdateModel.HasDesign)
                {
                    var renders = await Db.RenderModels.Where(p => p.AmputationTypeId == orderUpdateModel.AmputationTypeFkId).ToListAsync();
                    foreach (var render in renders)
                    {
                        var renderPieces = await Db.RenderPieceModels.Where(p => p.RenderId == render.Id).ToListAsync();

                        if (renderPieces == null || renderPieces.Count == 0) continue;

                        foreach (var piece in renderPieces)
                        {
                            orderModel.RenderPieces.Add(new OrderRenderPieceModel
                            {
                                RenderPieceId = piece.Id,
                                Printed = false
                            });
                        }
                    }
                }
            }

            orderModel.LogMessage(User, "Edited order by user", olderModelStr);
            Db.OrderModels.AddOrUpdate(orderModel);

            await Db.SaveChangesAsync();

            return Json(new
            {
                Action = Url.Action("Details", "Orders", new { id = orderModel.Id })
            });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteImage(OrderDeleteImage model)
        {
            if (!ModelState.IsValid) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var orderModel = await Db.OrderModels.FindAsync(model.OrderId);
            if (orderModel == null) return HttpNotFound();

            var resultRemoveFile = await _userFiles.RemoveImageAsync(model.FileNameBlob);

            if (resultRemoveFile)
            {
                string newImageStr = string.Empty;

                var images = orderModel.IdImage.Split(',');

                foreach (var image in images)
                {
                    var imageName = image.Split('/').LastOrDefault();

                    if (imageName != model.FileNameBlob)
                        newImageStr += $"{image},";
                }

                if (newImageStr.EndsWith(","))
                    newImageStr = newImageStr.Remove(newImageStr.Length - 1);

                orderModel.IdImage = newImageStr;
            }

            Db.OrderModels.AddOrUpdate(orderModel);
            await Db.SaveChangesAsync();

            return Json(new
            {
                IsSuccesful = resultRemoveFile
            });
        }

        // GET: Orders/Index
        public ActionResult Index()
        {
            return RedirectToActionPermanent("Index", "Home");
        }

        // GET: Orders/Details/5
        [OverrideAuthorize(Roles = AppRoles.User + ", " + AppRoles.Administrator)]
        public async Task<ActionResult> Details(int? id)
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderModel = await Db.OrderModels.Include(p => p.RenderPieces.Select(x => x.RenderPiece.Render.Pieces)).Include(x => x.OrderRequestor).Include(p => p.AmputationTypeFk).Include(p => p.ColorFk).FirstOrDefaultAsync(x => x.Id == id);

            if (orderModel == null)
            {
                return HttpNotFound();
            }
            if (!orderModel.CanView(User))
            {
                return RedirectToAction("RedirectUser", "Account");
            }

            var orderModelReturn = Mapper.Map<OrderDetailsViewModel>(orderModel);

            return View(orderModelReturn);
        }

        // GET: Orders/ManoPedir
        public async Task<ActionResult> ManoPedir()
        {
            var userId = User.Identity.GetUserId();
            var userModel = Db.UserModelsT.SingleOrDefault(x => x.UserId == userId);

            if (userModel == null) return RedirectToAction("Login", "Account");

            if (!userModel.IsValidAge())
            {
                return RedirectToAction("Index", "Users");
            }

            var listAmputations = await Db.AmputationTypeModels.ToListAsync();
            var listAmputationDesign = new List<AmputationDesign>();

            foreach (var amputation in listAmputations)
            {
                var hasDesign = await Db.RenderModels.AnyAsync(p => p.AmputationTypeId == amputation.Id);

                listAmputationDesign.Add(new AmputationDesign
                {
                    Amputation = amputation,
                    HasDesign = hasDesign
                });
            }

            ViewData["ListAmputations"] = listAmputationDesign;

            return View("ManoPedir", new NewOrder());
        }

        // POST: Orders/ManoPedir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManoPedir(NewOrder orderModel)
        {
            TempData["AmputationType"] = orderModel.AmputationTypeFkId;
            TempData["ProductType"] = orderModel.ProductType;
            var hasDesign = await Db.RenderModels.AnyAsync(p => p.AmputationTypeId == orderModel.AmputationTypeFkId);

            TempData["hasDesign"] = hasDesign;

            return RedirectToAction("ManoImagen");
        }

        // GET: Orders/ManoImagen
        public ActionResult ManoImagen()
        {
            var amputationType = TempData["AmputationType"];
            var productType = TempData["ProductType"];
            var hasDesign = TempData["hasDesign"];

            if (amputationType == null || productType == null)
            {
                return RedirectToAction("ManoPedir");
            }

            return View("ManoImagen", new NewOrder
            {
                AmputationTypeFkId = (int)amputationType,
                ProductType = (ProductType)productType,
                HasDesign = (bool)hasDesign
            });
        }

        // POST: Orders/UploadImageUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult UploadImageUser(OrderModel orderModel, HttpPostedFileBase file)
        public ActionResult UploadImageUser(NewOrder orderModel, List<HttpPostedFileBase> file)
        {
            int v_maxFiles = 6;

            TempData["AmputationType"] = orderModel.AmputationTypeFkId;
            TempData["ProductType"] = orderModel.ProductType;
            TempData["hasDesign"] = orderModel.HasDesign;


            if (file == null || file.Count == 0)
                ModelState.AddModelError("nofile", @"Seleccione una foto.");
            else
            {
                if (file.Count > v_maxFiles)
                    ModelState.AddModelError("maxfiles", @"Supero el maximo de archivos permitidos.");

                foreach (var img in file)
                {
                    if (img.ContentLength > 1000000 * 5)
                        ModelState.AddModelError("bigfile", @"La foto elegida es muy grande (max = 5 MB).");

                    if (!img.IsImage())
                        ModelState.AddModelError("noimage", @"El archivo seleccionado no es una imagen.");
                }
            }
            if (!ModelState.IsValid)
            {
                return Json(new { Action = "ManoImagen" });
            }

            System.Uri fileUrl = null;
            List<string> filesUrl = new List<string>();
            foreach (var img in file)
            {
                var fileName = Guid.NewGuid().ToString("N") + ".jpg";

                fileUrl = _userFiles.UploadOrderFile(img.InputStream, fileName);
                filesUrl.Add(fileUrl.AbsoluteUri);
            }

            var filesUser = string.Join(",", filesUrl);
            TempData["fileUrl"] = filesUser;

            //AB 20171216: las medidas las toma el embajador
            //return Json(new { Action = "ManoMedidas" });
            return Json(new { Action = "ManoOrden" });
        }

        // GET: Orders/ManoOrden
        public async Task<ActionResult> ManoOrden()
        {
            int? amputationType = (int?)TempData["AmputationType"];

            if (amputationType == null) return RedirectToAction("ManoPedir");

            ViewData["renderColors"] = await Db.ColorModels.Where(p => p.AmputationTypeId == amputationType).ToListAsync();

            return View("ManoOrden", new NewOrder
            {
                IdImage = TempData["fileUrl"].ToString(),
                AmputationTypeFkId = Convert.ToInt32(amputationType),
                ProductType = (ProductType)TempData["ProductType"],
                HasDesign = (bool)TempData["hasDesign"]
            });
        }

        //AB 20171216: las medidas las toma el embajador
        //// GET: Orders/ManoMedidas
        //[OverrideAuthorize(Roles = AppRoles.Ambassador + ", " + AppRoles.Administrator)]
        [OverrideAuthorize(Roles = AppRoles.Administrator)]
        public async Task<ActionResult> ManoMedidas(int? orderId)
        {
            if (orderId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderModel = await Db.OrderModels.FindAsync(orderId.Value);

            if (orderModel == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            if (!User.IsInRole(AppRoles.Administrator))
            {
                bool ambassadorEditOrder = orderModel.OrderAmbassador.UserId == userId;

                if (orderModel.Status != OrderStatus.Pending)
                    return View("Index"); //Aca retornar el index del rol

                if (!ambassadorEditOrder)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }

            return View("ManoMedidas", orderModel);
        }

        [OverrideAuthorize(Roles = AppRoles.Ambassador + ", " + AppRoles.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateSize(OrderModel orderModelSize)
        {
            var orderId = orderModelSize.Id;

            var orderModel = await Db.OrderModels.FindAsync(orderId);

            if (orderModel == null)
            {
                return HttpNotFound();
            }

            if (orderModel.Status != OrderStatus.Pending)
                return View("Index"); //Aca retornar el index del rol

            var userId = User.Identity.GetUserId();
            if (!User.IsInRole(AppRoles.Administrator))
            {
                bool ambassadorEditOrder = orderModel.OrderAmbassador.UserId == userId;

                if (!ambassadorEditOrder)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }

            if (string.IsNullOrWhiteSpace(orderModel.IdImage))
                ModelState.AddModelError("noimage", @"Error desconocido, vuelva a comenzar.");
            if (orderModelSize.Sizes.A <= 0 || orderModelSize.Sizes.B <= 0 || orderModelSize.Sizes.C <= 0)
                ModelState.AddModelError("nodistance", @"Seleccione las medidas.");

            if (!ModelState.IsValid) return View("ManoMedidas", orderModel);


            orderModel.Sizes = orderModelSize.Sizes;

            Db.OrderModels.AddOrUpdate(orderModel);
            orderModel.LogMessage(User, "Add or update sizes");

            await Db.SaveChangesAsync();

            return RedirectToAction("Details", "Orders", new { id = orderId });
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NewOrder orderModel)
        {
            if (!ModelState.IsValid) return View("ManoOrden", orderModel);

            var currentUserId = User.Identity.GetUserId();
            var userModel = await Db.UserModelsT.SingleAsync(c => c.UserId == currentUserId);

            var newOrder = Mapper.Map<OrderModel>(orderModel);

            newOrder.OrderRequestor = userModel;
            newOrder.LogMessage(User, "New order");

            if (!orderModel.HasDesign)
            {
                Db.OrderModels.Add(newOrder);

                await Db.SaveChangesAsync();
                await _ns.SendNewOrderNotificacion(newOrder);

                return RedirectToAction("Index", "Users");
            }

            var renders = await Db.RenderModels.Where(p => p.AmputationTypeId == newOrder.AmputationTypeFkId).ToListAsync();
            foreach (var render in renders)
            {
                var renderPieces = await Db.RenderPieceModels.Where(p => p.RenderId == render.Id).ToListAsync();

                if (renderPieces != null && renderPieces.Count > 0)
                {
                    foreach (var piece in renderPieces)
                    {
                        newOrder.RenderPieces.Add(new OrderRenderPieceModel
                        {
                            RenderPieceId = piece.Id,
                            Printed = false
                        });
                    }
                }
            }

            Db.OrderModels.Add(newOrder);
            await Db.SaveChangesAsync();

            await _ns.SendNewOrderNotificacion(newOrder);

            return RedirectToAction("Index", "Users");
        }

        // GET: Orders/GetUserImage
        [OverrideAuthorize(Roles = AppRoles.User + ", " + AppRoles.Administrator)]
        public ActionResult GetUserImage(string url)
        {
            var client = new HttpClient();

            var file = client.GetByteArrayAsync(url);

            return new FileContentResult(file.Result, "image/jpeg");
        }


        // POST: Orders/UploadProofOfDelivery
        [HttpPost]
        [OverrideAuthorize(Roles = AppRoles.User + ", " + AppRoles.Administrator)]
        public async Task<ActionResult> UploadProofOfDelivery(HttpPostedFileBase file, int orderId)
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
            var orderModel = await Db.OrderModels.Include(x => x.OrderRequestor).FirstOrDefaultAsync(x => x.Id == orderId);
            if (orderModel == null) return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            if (!orderModel.CanView(User))
            {
                return RedirectToAction("RedirectUser", "Account");
            }
            if (!ModelState.IsValid) return View("Details", orderModel);


            var fileName = Guid.NewGuid().ToString("N") + ".jpg";
            var fileUrl = _userFiles.UploadOrderFile(file?.InputStream, fileName);

            orderModel.ProofOfDelivery = fileUrl.AbsoluteUri;
            orderModel.LogMessage(User, "New proof of delivery at: " + fileUrl.AbsoluteUri);
            Db.SaveChanges();

            await _ns.SendProofOfDeliveryNotification(orderModel);

            //return Redirect(Request.UrlReferrer?.PathAndQuery);
            return RedirectToAction("Details", new { orderModel.Id });
        }

        [HttpPost]
        [OverrideAuthorize(Roles = AppRoles.Ambassador + ", " + AppRoles.Administrator)]
        public async Task<ActionResult> PrintedPiecesUpdate(List<RenderPieceGroupByViewModel> RenderPiecesGroupBy, int orderId)
        {
            var order = await Db.OrderModels.FirstOrDefaultAsync(x => x.Id == orderId);
            if (!order.CanView(User)) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            order.StatusLastUpdated = DateTime.UtcNow;
            Db.OrderModels.AddOrUpdate(order);

            IEnumerable<OrderRenderPieceModel> renderPieces = new List<OrderRenderPieceModel>();

            foreach (var renderPieceGroupBy in RenderPiecesGroupBy)
                renderPieces = renderPieces.Concat(Mapper.Map<IEnumerable<OrderRenderPieceModel>>(renderPieceGroupBy.OrderRenderPieces));

            Db.OrderRenderPieceModels.AddOrUpdate(renderPieces.ToArray());
            await Db.SaveChangesAsync();

            if (Request.IsAjaxRequest())
                return new HttpStatusCodeResult(HttpStatusCode.OK);

            return RedirectToAction("Details", "Orders", new { id = orderId });
        }

        [OverrideAuthorize(Roles = AppRoles.Ambassador + ", " + AppRoles.Administrator)]
        public async Task<ActionResult> GetPartial(int orderId, string partialName)
        {
            var orderModel = await Db.OrderModels.Include(p => p.RenderPieces.Select(x => x.RenderPiece.Render.Pieces)).Include(x => x.OrderRequestor).Include(p => p.AmputationTypeFk).Include(p => p.ColorFk).FirstOrDefaultAsync(x => x.Id == orderId);
            if (orderModel == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var orderMap = Mapper.Map<OrderDetailsViewModel>(orderModel);

            if (orderModel.CanView(User))
            {
                return PartialView("Details/_" + partialName, orderMap);
            }
            return new HttpUnauthorizedResult();
        }

        [AllowAnonymous]
        public ActionResult PublicOrders()
        {
            var orders = Db.OrderModels.Include(c => c.OrderRequestor).Include(c => c.OrderAmbassador).OrderByDescending(x => x.Date).ToList();
            return View(orders);
        }

        [OverrideAuthorize(Roles = AppRoles.Ambassador + ", " + AppRoles.Administrator)]
        public async Task<ActionResult> ProductGenerate(int id)
        {
            var order = await Db.OrderModels.FirstOrDefaultAsync(x => x.Id == id);

            if (order == null || !order.CanView(User))
            {
                return HttpNotFound();
            }

            await AzureQueue.EnqueueAsync(new OrderProductGenerator
            {
                OrderId = order.Id,
                Pieces = order.Pieces,
                ProductSizes = order.Sizes,
            });

            order.FileUrl = null;
            await Db.SaveChangesAsync();

            TempData["Generating"] = true;

            return RedirectToAction("Details", new { id });
        }

        [OverrideAuthorize(Roles = AppRoles.Ambassador + ", " + AppRoles.Administrator)]
        public async Task<ActionResult> ProductGenerated(int id, string fileurl)
        {
            var order = await Db.OrderModels.FirstOrDefaultAsync(x => x.Id == id);

            if (order == null || !order.CanView(User))
            {
                return HttpNotFound();
            }

            return order.FileUrl != fileurl ?
                new HttpStatusCodeResult(HttpStatusCode.Created) :
                new HttpStatusCodeResult(HttpStatusCode.NotModified);
        }
    }
}