using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Limbs.Web.Models;
using Limbs.Web.Repositories;
using Microsoft.AspNet.Identity;
using Limbs.Web.Controllers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;


namespace Limbs.Web.Controllers
{


    ///se comentan los repository para el funcionamiento del front
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
      
        //  public IOrdersRepository OrdersRepository { get; set; }

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
            return View("pedir_mano_index");
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
                orderModel.OrderRequestor = userModel;

                //Asigno ambassador
                orderModel.OrderAmbassador = MatchWithAmbassador(userModel);
                orderModel.Status = OrderStatus.PreAssigned;
                orderModel.StatusLastUpdated = DateTime.Now;
                orderModel.Date = DateTime.Now; 
                //asocio idImage a order
                var idLastOrder = db.OrderModels.Max(a => a.Id);
                idLastOrder ++;
                var nameFile = orderModel.OrderRequestor.Email + "-" + idLastOrder;
                
                //Proceso foto
                Upload_file(file,nameFile);
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

            //devolver los embajadores donde su id aparezca menos de 3 veces en las ordenes
            // var cant = QuantityOrders(ambassador);
            //  var ambassadors2 = db.AmbassadorModels.Where(a => 3 > QuantityOrders(a)).ToList();

            //var ambassadors = db.AmbassadorModels.Where(a => 10000 > db.OrderModels.Where(o => o.OrderAmbassador.Id == a.Id).Count()).ToList();
            //descomentar la linea de abajo y comentar la de arriba desp de testeos
            AmbassadorModel ambassador = db.AmbassadorModels.First();
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

        public void Upload_file(HttpPostedFileBase file,string name)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    //string path = Path.Combine(Server.MapPath("~/Content/img/Upload"), Path.GetFileName(file.FileName));
                    string path = Path.Combine(Server.MapPath("~/Content/img/Upload"), name+Path.GetExtension(file.FileName));
                    //define credential
                    UserCredential credential = GetUserCredential();

                    //define service 
                    DriveService service = GetDriveService(credential);

                    //ListFiles
                    //ListFiles(service);   

                    //saveFile
                    file.SaveAs(path);
                    //uploadFile
                    UploadFileDrive(path, service,name);
                
                }
                catch (Exception ex)
                {
                }
            else
            {
            }
        }

        //-----------------------------Google drive api------------------------//

        private static void ListFiles(DriveService service)
        {
            //define files
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    //muestro atributos
                }
            }
            else
            {
                //no hay archivos
            }
        }


        private static void UploadFileDrive(string path, DriveService service,string name)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name
            };
            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(path,
                            System.IO.FileMode.Open))
            {
                request = service.Files.Create(
                fileMetadata, stream, "image/jpeg");
                request.Fields = "id";
                request.Upload();
            }
            var file = request.ResponseBody;
        }

        private static UserCredential GetUserCredential()
        {
            string[] Scopes = { DriveService.Scope.DriveFile };

            //var client_json = Server.MapPath("/Scripts/client_secret.json"); --se usa con el choclo de abajo porque GetUserCredential es static--
            var client_json = System.Web.HttpContext.Current.Server.MapPath("/Scripts/client_secret.json");

            using (var stream =
                new FileStream(client_json, FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
        }

        private static DriveService GetDriveService(UserCredential credential)
        {
            var applicationName = "Limbs";
            // Create Drive API service.
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });
        }
        //--------------------------------------------------------------------//

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
        public ActionResult pedir_mano_index()
        {
            return View();
        }

        [Authorize(Roles = "Requester")]
        public ActionResult pedir_brazo_medidas()
        {
            return View();
        }

        [Authorize(Roles = "Requester")]
        public ActionResult pedir_mano_medidas()
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