using System;
using System.Data.Entity;
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

            foreach (var data in dataList)
            {
                string ambassadorText = data.ToString();
                sb.AppendLine(ambassadorText);
            }

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var nameCsv = String.Format("embajadores{0}.csv", Timestamp);
            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", nameCsv);
                    
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