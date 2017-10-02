using System.Web.Mvc;
using Limbs.Web.Models;

namespace Limbs.Web.Controllers
{
    public class BaseController : Controller
    {
        public ApplicationDbContext Db = new ApplicationDbContext();

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