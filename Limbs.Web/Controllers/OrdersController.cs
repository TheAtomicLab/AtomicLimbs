using Limbs.Web.Common.Extensions;
using Limbs.Web.Entities.Models;
using Limbs.Web.Repositories.Interfaces;
using Limbs.Web.Services;
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
using Limbs.Web.Common.Mail;
using Microsoft.AspNet.Identity.Owin;

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

        // GET: Orders/Index
        public ActionResult Index()
        {
            return RedirectToActionPermanent("Index", "Home");
        }

        // GET: Orders/Details/5
        [OverrideAuthorize(Roles = AppRoles.User + ", " + AppRoles.Administrator)]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderModel = await Db.OrderModels.Include(x => x.OrderRequestor).FirstOrDefaultAsync(x => x.Id == id);
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            if (!orderModel.CanView(User))
            {
                return RedirectToAction("RedirectUser", "Account");
            }
            return View(orderModel);
        }

        // GET: Orders/ManoPedir
        public ActionResult ManoPedir()
        {
            var userId = User.Identity.GetUserId();
            var userModel = Db.UserModelsT.SingleOrDefault(x => x.UserId == userId);

            if (userModel == null) return RedirectToAction("Login", "Account");

            if (!userModel.IsValidAge())
            {
                return RedirectToAction("Index", "Users");
            }
            return View("ManoPedir", new OrderModel());
        }


        // POST: Orders/ManoPedir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManoPedir(OrderModel orderModel)
        {
            TempData["AmputationType"] = orderModel.AmputationType;
            TempData["ProductType"] = orderModel.ProductType;

            var viewName = "ManoImagen";
            if (orderModel.AmputationType.EsBrazo())
            {
                viewName = "BrazoImagen";
            }
            return RedirectToAction(viewName);
        }

        // GET: Orders/ManoImagen
        public ActionResult ManoImagen()
        {
            var amputationType = TempData["AmputationType"];
            var productType = TempData["ProductType"];
            if (amputationType == null || productType == null)
            {
                return RedirectToAction("ManoPedir");
            }

            return View("ManoImagen", new OrderModel
            {
                AmputationType = (AmputationType)amputationType,
                ProductType = (ProductType)productType,
            });
        }


        // GET: Orders/BrazoImagen
        public ActionResult BrazoImagen()
        {
            var amputationType = TempData["AmputationType"];
            var productType = TempData["ProductType"];
            if (amputationType == null || productType == null)
            {
                return RedirectToAction("ManoPedir");
            }

            return View("BrazoImagen", new OrderModel
            {
                AmputationType = (AmputationType)amputationType,
                ProductType = (ProductType)productType,
            });
        }

        // POST: Orders/UploadImageUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadImageUser(OrderModel orderModel, HttpPostedFileBase file)
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
            if (!ModelState.IsValid)
            {
                TempData["AmputationType"] = orderModel.AmputationType;
                TempData["ProductType"] = orderModel.ProductType;

                return Json(new { Action = "ManoImagen" });
            }

            var fileName = Guid.NewGuid().ToString("N") + ".jpg";
            var fileUrl = _userFiles.UploadOrderFile(file?.InputStream, fileName);

            TempData["fileUrl"] = fileUrl.AbsoluteUri;
            TempData["AmputationType"] = orderModel.AmputationType;
            TempData["ProductType"] = orderModel.ProductType;

            //AB 20171216: las medidas las toma el embajador
            //return Json(new { Action = "ManoMedidas" });
            return Json(new { Action = "ManoOrden" });
        }

        // GET: Orders/ManoOrden
        public ActionResult ManoOrden()
        {
            return View("ManoOrden", new OrderModel
            {
                IdImage = TempData["fileUrl"].ToString(),
                AmputationType = (AmputationType)TempData["AmputationType"],
                ProductType = (ProductType)TempData["ProductType"],
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

            if (orderModel.Status != OrderStatus.Pending)
                return View("Index"); //Aca retornar el index del rol

            var userId = User.Identity.GetUserId();
            if (!User.IsInRole(AppRoles.Administrator))
            {
                bool ambassadorEditOrder = orderModel.OrderAmbassador.UserId == userId;

                if (!ambassadorEditOrder)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }

            return View("ManoMedidas", orderModel);
        }

        //// POST: Orders/ManoOrden
        //[HttpPost] 
        //[ValidateAntiForgeryToken] 
        //public ActionResult ManoOrden(OrderModel orderModel) 
        //{ 
        //    ModelState.Clear(); 
        //    if (string.IsNullOrWhiteSpace(orderModel.IdImage)) 
        //        ModelState.AddModelError("noimage", @"Error desconocido, vuelva a comenzar."); 
        //    if (orderModel.Sizes.A <= 0 || orderModel.Sizes.B <= 0 || orderModel.Sizes.C <= 0) 
        //        ModelState.AddModelError("nodistance", @"Seleccione las medidas."); 
        // 
        //    if (!ModelState.IsValid) return View("ManoMedidas", orderModel); 
        // 
        //    return View("ManoOrden", orderModel); 
        //} 

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
        public async Task<ActionResult> Create(OrderModel orderModel)
        {
            if (!ModelState.IsValid) return View("ManoOrden", orderModel);

            var currentUserId = User.Identity.GetUserId();
            var userModel = await Db.UserModelsT.Where(c => c.UserId == currentUserId).SingleAsync();

            orderModel.OrderRequestor = userModel;
            orderModel.Status = OrderStatus.NotAssigned;
            orderModel.StatusLastUpdated = DateTime.UtcNow;
            orderModel.Date = DateTime.UtcNow;
            orderModel.LogMessage(User, "New order");

            Db.OrderModels.Add(orderModel);
            await Db.SaveChangesAsync();

            //AB 20171216: las medidas las toma el embajador
            //await AzureQueue.EnqueueAsync(new OrderProductGenerator
            //{
            //    OrderId = orderModel.Id,
            //    Pieces = orderModel.Pieces,
            //    ProductSizes = orderModel.Sizes,
            //});

            await _ns.SendNewOrderNotificacion(orderModel);

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
            return RedirectToAction("Details",new { orderModel.Id });
        }

        [HttpPost]
        [OverrideAuthorize(Roles = AppRoles.Ambassador + ", " + AppRoles.Administrator)]
        public async Task<ActionResult> PrintedPiecesUpdate(Pieces pieces, int orderId)
        {
            var order = await Db.OrderModels.FirstOrDefaultAsync(x => x.Id == orderId);
            if (!order.CanView(User)) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            order.Pieces = pieces;
            order.StatusLastUpdated = DateTime.UtcNow;
            Db.OrderModels.AddOrUpdate(order);
            await Db.SaveChangesAsync();

            if (Request.IsAjaxRequest())
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            return RedirectToAction("Details", "Orders", new { id = orderId });
        }

        [OverrideAuthorize(Roles = AppRoles.Ambassador + ", " + AppRoles.Administrator)]
        public async Task<ActionResult> GetPartial(int orderId, string partialName)
        {
            var order = await Db.OrderModels.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order.CanView(User))
            {
                return PartialView("Details/_" + partialName, order);
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