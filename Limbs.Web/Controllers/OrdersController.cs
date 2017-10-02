using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Limbs.Web.Extensions;
using Limbs.Web.Models;
using Microsoft.AspNet.Identity;
using Limbs.Web.Repositories.Interfaces;

namespace Limbs.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        //  public IOrdersRepository OrdersRepository { get; set; }
        private readonly IUserFiles _userFiles;
        
        public OrdersController(IUserFiles userFiles)
        {
            _userFiles = userFiles;
        }

        // GET: Orders/Index
        public ActionResult Index()
        {
            return RedirectToActionPermanent("Index", "Home");
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderModel = await _db.OrderModels.Include(x => x.OrderRequestor).FirstOrDefaultAsync(x => x.Id == id);
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            if (!CanViewOrder(orderModel))
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        private bool CanViewOrder(OrderModel order)
        {
            if (User.IsInRole("Administrator")) return true;

            //check ownership
            return order.OrderAmbassador.UserId == User.Identity.GetUserId() ||
                   order.OrderRequestor.UserId == User.Identity.GetUserId();
        }

        // GET: Orders/ManoPedir
        [Authorize(Roles = "Requester")]
        public ActionResult ManoPedir()
        {
            return View();
        }

        // POST: Orders/UploadImageUser
        [Authorize(Roles = "Requester")]
        public ActionResult UploadImageUser(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                ModelState.AddModelError("nofile", "Seleccione una foto.");
            }
            else
            {
                if (file.ContentLength > 1000000 * 5)
                {
                    ModelState.AddModelError("bigfile", "La foto elegida es muy grande (max = 5 MB).");
                }
                if (!file.IsImage())
                {
                    ModelState.AddModelError("noimage", "El archivo seleccionado no es una imagen.");
                }
            }
            if (!ModelState.IsValid) return View("ManoPedir");


            var fileName = Guid.NewGuid().ToString("N") + ".jpg";
            var fileUrl = _userFiles.UploadOrderFile(file.InputStream, fileName);

            TempData["fileUrl"] = Url.Action("GetUserImage", "Users", new { url = fileUrl.AbsoluteUri });

            return RedirectToAction("ManoMedidas");
        }

        // GET: Orders/ManoMedidas
        [Authorize(Roles = "Requester")]
        public ActionResult ManoMedidas()
        {
            if (TempData["fileUrl"] == null)
            {
                return RedirectToAction("ManoPedir", "Orders");
            }

            ViewBag.ImageUrl = TempData["fileUrl"];
            return View("ManoMedidas");
        }

        // POST: Orders/ManoOrden
        [HttpPost]
        [Authorize(Roles = "Requester")]
        public ActionResult ManoOrden(string imageUrl, float distA, float distB, float distC)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                ModelState.AddModelError("noimage", "Error desconocido, vuelva a comenzar.");
            if (distA <= 0 || distB <= 0 || distC <= 0)
                ModelState.AddModelError("nodistance", "Seleccione las medidas.");
            ViewBag.ImageUrl = imageUrl;
            if (!ModelState.IsValid) return View("ManoMedidas");

            return View("ManoOrden", new OrderModel
            {
                IdImage = imageUrl,
                Sizes = new OrderSizesModel
                {
                    A = distA,
                    B = distB,
                    C = distC,
                }
            });
        }

        // POST: Orders/Create
        [Authorize(Roles = "Requester")]
        [HttpPost]
        public async Task<ActionResult> Create(OrderModel orderModel)
        {
            if (!ModelState.IsValid) return View("ManoOrden", orderModel);


            var currentUserId = User.Identity.GetUserId();
            // Consulta DB. Cambiar con repos
            var userModel = await _db.UserModelsT.Where(c => c.UserId == currentUserId).SingleAsync();
            
            //TODO (ale): revisar logica, porque no lo validamos en paso 1?
            //var hasPendingOrders = await _db.OrderModels.Where(c => c.OrderRequestor.Id == userModel.Id && c.Status != OrderStatus.Delivered).CountAsync();
            //if (hasPendingOrders > 1)
            //    return RedirectToAction("UserPanel", "Users", new { message = "Cantidad de pedidos excedidos" });

            orderModel.OrderRequestor = userModel;
            orderModel.Status = OrderStatus.NotAssigned;
            orderModel.StatusLastUpdated = DateTime.UtcNow;
            orderModel.Date = DateTime.UtcNow;
            //Asigno ambassador (ale: primer version, la asignacion la hacemos nosotros)
            //orderModel.OrderAmbassador = MatchWithAmbassador(userModel);

            _db.OrderModels.Add(orderModel);
            await _db.SaveChangesAsync();

            return RedirectToAction("UserPanel", "Users");
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}