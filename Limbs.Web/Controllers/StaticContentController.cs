using System.Web.Mvc;

namespace Limbs.Web.Controllers
{
    public class StaticContentController : Controller
    {
        // GET: StaticContent/PageNotFound
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            if (Request.IsAjaxRequest())
                return new HttpStatusCodeResult(404);
            return View();
        }

        // GET: StaticContent/Error
        public ActionResult Error()
        {
            Response.StatusCode = 500;
            if(Request.IsAjaxRequest())
                return new HttpStatusCodeResult(500);
            return View("Error");
        }
    }
}