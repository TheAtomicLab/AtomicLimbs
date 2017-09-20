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
using System.IO;

namespace Limbs.Web.Controllers
{


    ///se comentan los repository para el funcionamiento del front
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //  public IOrdersRepository OrdersRepository { get; set; }
        private readonly IUserFiles _userFiles;
        
        public OrdersController(IUserFiles userFiles)
        {
            _userFiles = userFiles;
        }

        // GET: Orders/Index
        public ActionResult Index()
        {
            return View();

        }

        // GET: Orders/Create
        [Authorize(Roles = "Requester")]
        public ActionResult Create()
        {
            return View("Create");
        }

        [Authorize(Roles = "Requester")]
        public ActionResult CreateHand()
        {
            return View("PedirProtesis");
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Requester")]
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Id,Design,Sizes,Comments")] OrderModel orderModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity.GetUserId();
                // Consulta DB. Cambiar con repos
                var userModel = await db.UserModelsT.Where(c => c.UserId == currentUserId).SingleAsync();
                var hasPendingOrders = await db.OrderModels.Where(c => c.OrderRequestor.Id == userModel.Id && c.Status != OrderStatus.Delivered).CountAsync();
                if (hasPendingOrders > 1)
                    return RedirectToAction("UserPanel", "Users", new { message = "Cantidad de pedidos excedidos" });
                

                orderModel.OrderRequestor = userModel;

                //Asigno ambassador
                orderModel.OrderAmbassador = MatchWithAmbassador(userModel);
                orderModel.Status = OrderStatus.PreAssigned;
                orderModel.StatusLastUpdated = DateTime.Now;
                orderModel.Date = DateTime.Now;

                //TODO (ale): refactor image logic
                //asocio idImage a order
                int idLastOrder = 0;

                if (db.OrderModels.Count() > 0)
                {
                    idLastOrder = db.OrderModels.Max(a => a.Id);
                    idLastOrder++;
                }

                var nameFile = orderModel.OrderRequestor.Email + "-" + idLastOrder;

                //Proceso foto
                if (file != null && file.ContentLength > 0)
                {
                    _userFiles.UploadOrderFile(file.InputStream, nameFile);
                }
                orderModel.IdImage = nameFile;

                db.OrderModels.Add(orderModel);
                await db.SaveChangesAsync();

                return RedirectToAction("UserPanel", "Users");
            }

            return View(orderModel);
        }

        public double MatcheoWithAmbassador(UserModel user, AmbassadorModel ambassador)
        {
            var distance = GetDistance(user.Long, user.Lat, ambassador.Long, ambassador.Lat);
            return distance;
        }

        public int QuantityOrders(AmbassadorModel ambassador)
        {
            var idAmbassador = ambassador.Id;
            var cant = db.OrderModels.Where(o => o.OrderAmbassador.Id == idAmbassador).Count();
            return cant;
        }

        public AmbassadorModel MatchWithAmbassador(UserModel user)
        {
            if (db.AmbassadorModels.Count() > 0)
            {
                AmbassadorModel ambassador = db.AmbassadorModels.First();
                //devolver los embajadores donde su id aparezca menos de 3 veces en las ordenes
                // var cant = QuantityOrders(ambassador);
                //  var ambassadors2 = db.AmbassadorModels.Where(a => 3 > QuantityOrders(a)).ToList();

                //var ambassadors = db.AmbassadorModels.Where(a => 10000 > db.OrderModels.Where(o => o.OrderAmbassador.Id == a.Id).Count()).ToList();
                //descomentar la linea de abajo y comentar la de arriba desp de testeos
                var ambassadors = db.AmbassadorModels.Where(a => 3 > db.OrderModels.Where(o => o.OrderAmbassador.Id == a.Id).Count()).ToList();

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

                    if (distance1 > distance2)
                    {
                        if (distance2 < 500000)
                        {
                            ambassadorSelect = ambassador2;
                        }
                    }
                }

                if (ambassadorSelect != null)
                {
                    //alerta para el usuario de que matcheo
                    return ambassadorSelect;
                }
                else
                {
                    //alerta para el usuario de que no matcheo con ningun embajador
                    return null;
                }
            }
            else
            {
                //si entro aca no hay embajadores
                return null;
            }
        }

        private static double GetDistance(double long1InDegrees, double lat1InDegrees, double long2InDegrees, double lat2InDegrees)
        {
            double lats = (double)(lat1InDegrees - lat2InDegrees);
            double lngs = (double)(long1InDegrees - long2InDegrees);

            //Paso a metros
            double latm = lats * 60 * 1852;
            double lngm = (lngs * Math.Cos((double)lat1InDegrees * Math.PI / 180)) * 60 * 1852;
            double distInMeters = Math.Sqrt(Math.Pow(latm, 2) + Math.Pow(lngm, 2));
            return distInMeters;
        }
        
        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            OrderModel orderModel;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var orderID = id.Value;

                // Consulta DB. Cambiar con repos
                orderModel = db.OrderModels.Find(orderID);

            }

            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Comments,Status")] OrderModel orderModel)
        {
            if (ModelState.IsValid)
            {

                //    _context.Entry(orderModel).State = EntityState.Modified;
                //    await _context.SaveChangesAsync();
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(orderModel);
        }

        // GET: Orders/Delete/5
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
                orderModel = db.OrderModels.Find(orderId);

            }
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        [Authorize(Roles = "Requester")]
        public ActionResult PedirProtesis()
        {
            return View();
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //  OrderModel orderModel = OrdersRepository.Get(id);
            OrderModel ordelModel = await db.OrderModels.FindAsync(id);
            db.OrderModels.Remove(ordelModel);
            //OrdersRepository.Remove(orderModel);
            //    await _context.SaveChangesAsync();
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Requester")]
        public ActionResult UploadImageUser( HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                byte[] data;
                using (Stream inputStream = file.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }
                TempData["fileData"] = data;
            }
            
            return RedirectToAction("ProtesisMedidas");

        }

        [Authorize(Roles = "Requester")]
        public ActionResult ProtesisMedidas()
        {
            ViewBag.ImageBytes = TempData["fileData"];
            return View("ProtesisMedidas");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}