using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class AmbassadorsController : AdminBaseController
    {
        // GET: Admin/Ambassadors
        public ActionResult Index()
        {
            return View(Db.AmbassadorModels.ToList());
        }

        // GET: Admin/Ambassadors/CsvExport
        public async Task<FileContentResult> CsvExport()
        {

            var dataList = await Db.AmbassadorModels.ToListAsync();
            var sb = new StringBuilder();

            var firstAmbassador = dataList.FirstOrDefault();
            var ambassadorExportTitles = firstAmbassador.GetTitles();
            sb.AppendLine(String.Join(",", ambassadorExportTitles));

            foreach (var ambassador in dataList)
            {
                string ambassadorText = ambassador.ToString();
                sb.AppendLine(ambassadorText);
            }

            var DateTimeExport = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            var nameCsv = String.Format("embajadores_{0}.csv", DateTimeExport);

            var data = Encoding.UTF8.GetBytes(sb.ToString());
            var result = Encoding.UTF8.GetPreamble().Concat(data).ToArray();

            return File(result, "text/csv", nameCsv);
                    
        }

        // GET: Admin/Ambassador/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ambassadorModel = await Db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }
            return View(ambassadorModel);
        }

        // GET: Admin/Ambassador/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ambassadorModel = await Db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }
            return View(ambassadorModel);
        }

        // POST: Admin/Ambassador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var ambassadorModel = await Db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }

            Db.AmbassadorModels.Remove(ambassadorModel);
            await Db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}