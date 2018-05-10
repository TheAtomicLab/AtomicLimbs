using System;
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
        public FileContentResult CsvExport()
        {

            var dataList = Db.AmbassadorModels.ToList();
            var sb = new StringBuilder();

            sb.AppendLine("Id, " + "Email, " + "Nombre, " + "DNI, " + "Pais, " + "Ciudad, " + "Domicilio, " + "Datos extra domicilio, " + "Estado, " + "Localizacion, " + "Telefono, " + "Fecha de registro, " + "Fecha de nacimiento, " + "Genero");
            foreach (var data in dataList)
            {

                sb.AppendLine(data.Id + "," + data.Email + ", " + data.FullName() + ", " + data.Dni + ", " +
                              data.Country + ", " + data.City + ", " + data.Address + ", " +
                              data.Address2.Replace(',', ' ') + ", " + data.State +  ", " + data.LatLng.Replace(',', ' ') + ", " +
                              data.Phone + ", " + data.RegisteredAt + ", " + data.Birth + ", " +
                              data.Gender);

            }

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "embajadores" + Timestamp + ".csv");

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