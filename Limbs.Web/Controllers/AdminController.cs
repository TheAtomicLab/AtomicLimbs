using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Limbs.Web.Models;
using System.Data.Entity;
using Limbs.Web.ViewModels;

namespace Limbs.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Orders()
        {
            //TODO (ale): implementar paginacion, export to excel, ordenamiento
            IEnumerable<OrderModel> orderList = db.OrderModels.Include(c => c.OrderRequestor).Include(c => c.OrderAmbassador).OrderBy(x => x.Date).ToList();
            return View(orderList);
        }


        public ActionResult OrderDetails(int id)
        {
            var order = db.OrderModels.Find(id);

            if(order == null)
                return HttpNotFound();

            return View(order);

        }

        public ActionResult EditOrder(int id)
        {
            var order = db.OrderModels.Find(id);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }




        public ActionResult Users()
        {
            //TODO (ale): implementar paginacion, export to excel, ordenamiento
            IEnumerable<UserModel> userList = db.UserModelsT.ToList();
            return View(userList);
        }

        public ActionResult UserDetails(int id)
        {
            var user = db.UserModelsT.Find(id);

            if (user == null)
                return HttpNotFound();

            return View(user);

        }
        public ActionResult EditUser(int id)
        {
            var user = db.UserModelsT.Find(id);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }




        public ActionResult Ambassadors()
        {
            //TODO (ale): implementar paginacion, export to excel, ordenamiento
            IEnumerable<AmbassadorModel> ambassadorList = db.AmbassadorModels.ToList();
            return View(ambassadorList);
        }

        public ActionResult AmbassadorDetails(int id)
        {
            var ambassador = db.AmbassadorModels.Find(id);

            if (ambassador == null)
                return HttpNotFound();

            return View(ambassador);

        }
        public ActionResult EditAmbassador(int id)
        {
            var ambassador = db.AmbassadorModels.Find(id);

            if (ambassador == null)
                return HttpNotFound();

            return View(ambassador);
        }

        public ActionResult SelectOrderAmbassador(int idOrder)
        {
            var order = db.OrderModels.Find(idOrder);

            if (order == null)
                return HttpNotFound();
            
            //TODO (ale): ordenar por distancia de la orden
            IEnumerable<AmbassadorModel> ambassadorList = db.AmbassadorModels.ToList();

            return View(new AssignOrderAmbassadorViewModel
            {
                Order = order,
                AmbassadorList = ambassadorList,
            });

        }

        public ActionResult AssignOrderAmbassador(int id, int idOrder)
        {
            var order = db.OrderModels.Find(idOrder);

            if (order == null)
                return HttpNotFound();
            
            var ambassador = db.AmbassadorModels.Find(id);

            if(ambassador == null)
                return HttpNotFound();

            order.OrderAmbassador = ambassador;
            order.Status = OrderStatus.PreAssigned;
            order.StatusLastUpdated = DateTime.UtcNow;
            db.SaveChanges();

            //TODO (ale): notificar a los usuarios el cambio de estado + asignacion

            return RedirectToAction("Orders");
        }
    }
}