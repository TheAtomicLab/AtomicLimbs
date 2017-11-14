using System;
using System.Globalization;
using System.Threading;
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

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-AR");

            return base.BeginExecuteCore(callback, state);
        }
    }
}