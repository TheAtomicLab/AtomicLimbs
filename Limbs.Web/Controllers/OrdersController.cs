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
    [DefaultAuthorize(Roles = AppRoles.Requester)]
    public class OrdersController : BaseController
    {
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
        [OverrideAuthorize(Roles = AppRoles.User)]
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
            if (!CanViewOrder(orderModel))
            {
                return RedirectToAction("RedirectUser", "Account");
            }
            return View(orderModel);
        }

        private bool CanViewOrder(OrderModel order)
        {
            if (User.IsInRole(AppRoles.Administrator)) return true;

            //check ownership
            if (order.OrderAmbassador != null)
                return order.OrderAmbassador.UserId == User.Identity.GetUserId();
            return order.OrderRequestor.UserId == User.Identity.GetUserId();
        }

        // GET: Orders/ManoPedir
        public ActionResult ManoPedir()
        {
            return View();
        }

        // POST: Orders/UploadImageUser
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
        [HttpPost]
        public async Task<ActionResult> Create(OrderModel orderModel)
        {
            if (!ModelState.IsValid) return View("ManoOrden", orderModel);


            var currentUserId = User.Identity.GetUserId();
            // Consulta DB. Cambiar con repos
            var userModel = await Db.UserModelsT.Where(c => c.UserId == currentUserId).SingleAsync();
            
            orderModel.OrderRequestor = userModel;
            orderModel.Status = OrderStatus.NotAssigned;
            orderModel.StatusLastUpdated = DateTime.UtcNow;
            orderModel.Date = DateTime.UtcNow;
            
            Db.OrderModels.Add(orderModel);
            await Db.SaveChangesAsync();

            return RedirectToAction("Index", "Users");
        }
    }
}