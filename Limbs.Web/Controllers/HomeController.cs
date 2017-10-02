using System.Web.Mvc;

namespace Limbs.Web.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("RedirectUser", "Account");
            }

            return View("Index");
        }
    }
}