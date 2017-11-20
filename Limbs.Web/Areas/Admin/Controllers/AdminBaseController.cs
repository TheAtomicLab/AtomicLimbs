using System.Web.Mvc;
using Limbs.Web.Entities.DbContext;
using Limbs.Web.Entities.Models;
using Limbs.Web.Models;

namespace Limbs.Web.Areas.Admin.Controllers
{
    [DefaultAuthorize(Roles = AppRoles.Administrator)]
    public class AdminBaseController : Controller
    {
        public readonly ApplicationDbContext Db = new ApplicationDbContext();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}