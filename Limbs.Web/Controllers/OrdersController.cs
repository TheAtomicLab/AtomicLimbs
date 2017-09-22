using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
            //TODO (ale): validacion de imagen, contenido, etc...
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Guid.NewGuid().ToString("N") + ".jpg";

                var fileUrl = _userFiles.UploadOrderFile(file.InputStream, fileName);

                TempData["fileUrl"] = Url.Action("GetUserImage", "Users", new { url = fileUrl.AbsoluteUri });
            }

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
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity.GetUserId();
                // Consulta DB. Cambiar con repos
                var userModel = await _db.UserModelsT.Where(c => c.UserId == currentUserId).SingleAsync();
                //TODO (ale): revisar logica, porque no lo validamos en paso 1?
                var hasPendingOrders = await _db.OrderModels.Where(c => c.OrderRequestor.Id == userModel.Id && c.Status != OrderStatus.Delivered).CountAsync();
                if (hasPendingOrders > 1)
                    return RedirectToAction("UserPanel", "Users", new { message = "Cantidad de pedidos excedidos" });
                

                orderModel.OrderRequestor = userModel;
                orderModel.Status = OrderStatus.PreAssigned;
                orderModel.StatusLastUpdated = DateTime.Now;
                orderModel.Date = DateTime.Now;
                //Asigno ambassador (ale: primer version, la asignacion la hacemos nosotros)
                //orderModel.OrderAmbassador = MatchWithAmbassador(userModel);

                _db.OrderModels.Add(orderModel);
                await _db.SaveChangesAsync();

                return RedirectToAction("UserPanel", "Users");
            }

            return View("ManoOrden", orderModel);
        }

        #region Match Ambassador
        public double MatcheoWithAmbassador(UserModel user, AmbassadorModel ambassador)
        {
            var distance = GetDistance(user.Long, user.Lat, ambassador.Long, ambassador.Lat);
            return distance;
        }

        public int QuantityOrders(AmbassadorModel ambassador)
        {
            var idAmbassador = ambassador.Id;
            var cant = _db.OrderModels.Count(o => o.OrderAmbassador.Id == idAmbassador);
            return cant;
        }

        public AmbassadorModel MatchWithAmbassador(UserModel user)
        {
            if (!_db.AmbassadorModels.Any()) return null;

            AmbassadorModel ambassador = _db.AmbassadorModels.First();
            //devolver los embajadores donde su id aparezca menos de 3 veces en las ordenes
            // var cant = QuantityOrders(ambassador);
            //  var ambassadors2 = db.AmbassadorModels.Where(a => 3 > QuantityOrders(a)).ToList();

            //var ambassadors = db.AmbassadorModels.Where(a => 10000 > db.OrderModels.Where(o => o.OrderAmbassador.Id == a.Id).Count()).ToList();
            //descomentar la linea de abajo y comentar la de arriba desp de testeos
            var ambassadors = _db.AmbassadorModels.Where(a => 3 > _db.OrderModels.Count(o => o.OrderAmbassador.Id == a.Id)).ToList();

            //var embajadoresConPedidosMenosDe3 = db.OrderModels.Where( o => o.OrderAmbassador).ToList();

            var distance1 = MatcheoWithAmbassador(user, ambassador);
            AmbassadorModel ambassadorSelect = null;
            if (distance1 < 500000 && ambassadors.Contains(ambassador))
            {
                ambassadorSelect = ambassador;
            }

            foreach (var ambassador2 in ambassadors)
            {
                var distance2 = MatcheoWithAmbassador(user, ambassador2);

                if (!(distance1 > distance2)) continue;
                if (distance2 < 500000)
                {
                    ambassadorSelect = ambassador2;
                }
            }

            //alerta para el usuario de que matcheo
            return ambassadorSelect;
            //si entro aca no hay embajadores
        }

        private static double GetDistance(double long1InDegrees, double lat1InDegrees, double long2InDegrees, double lat2InDegrees)
        {
            var lats = lat1InDegrees - lat2InDegrees;
            var lngs = long1InDegrees - long2InDegrees;

            //Paso a metros
            var latm = lats * 60 * 1852;
            var lngm = (lngs * Math.Cos(lat1InDegrees * Math.PI / 180)) * 60 * 1852;
            var distInMeters = Math.Sqrt(Math.Pow(latm, 2) + Math.Pow(lngm, 2));
            return distInMeters;
        }

        #endregion

        #region AdminActions
        // GET: Orders/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Consulta DB. Cambiar con repos
            var orderModel = _db.OrderModels.Find(id.Value);

            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Comments,Status")] OrderModel orderModel)
        {
            if (ModelState.IsValid)
            {

                //    _context.Entry(orderModel).State = EntityState.Modified;
                //    await _context.SaveChangesAsync();
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(orderModel);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            OrderModel orderModel;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var orderId = id.Value;

                // Consulta DB. Cambiar con repos
                orderModel = _db.OrderModels.Find(orderId);

            }
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // POST: Orders/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //  OrderModel orderModel = OrdersRepository.Get(id);
            var ordelModel = await _db.OrderModels.FindAsync(id);
            if (ordelModel != null)
            {
                _db.OrderModels.Remove(ordelModel);
                //OrdersRepository.Remove(orderModel);
                //    await _context.SaveChangesAsync();
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        #endregion

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