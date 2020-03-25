using Limbs.Web.ViewModels;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Limbs.Web.Controllers
{
    [AllowAnonymous, RoutePrefix("~/covid")]
    public class CovidController : BaseController
    {
        [HttpGet, Route]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, Route, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateCovidOrganizationViewModel model)
        {
            return RedirectToAction("Index", "Manage");
        }

    }
}

