using System;
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
            const string cultureName = "es-AR";
         
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }
    }
}