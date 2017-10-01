using System.Web.Mvc;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class PanelController : AdminBaseController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}