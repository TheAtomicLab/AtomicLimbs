using System.Web.Mvc;

namespace Limbs.Web.Controllers
{
    public class StaticContentController : Controller
    {
        // GET: StaticContent/faq
        public ActionResult faq()
        {
            return View();
        }
        // GET: StaticContent/ManualEmbajador
        public ActionResult ManualEmbajador()
        {
            return View();
        }

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

        // GET: StaticContent/Redirect
        public ActionResult Redirect()
        {
            Response.StatusCode = 200;
            return new HttpStatusCodeResult(200);
        }

    }
}