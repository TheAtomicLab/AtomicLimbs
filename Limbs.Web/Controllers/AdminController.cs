using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Limbs.Web.Models;
using System.Data.Entity;

namespace Limbs.Web.Controllers
{
    [Authorize(Roles = "Admin")]
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
            // Implementar paginación con repositorios
            IEnumerable<OrderModel> orderList = db.OrderModels.Include(c => c.OrderRequestor).Include(c => c.OrderAmbassador).ToList();
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
            // Implementar paginación con repositorios
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
            // Implementar paginación con repositorios
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

    }
}