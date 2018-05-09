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

            foreach (var data in dataList)
            {

                sb.AppendLine("Id: " + data.Id + "," + "Email: " + data.Email + ", " + "Nombre: " + data.FullName() + ", " + "DNI: " + data.Dni + ", " +
                              "Pais: " + data.Country + ", " + "Ciudad: " + data.City + ", " + "Domicilio: " + data.Address + ", " +
                              "Datos extra domicilio: " + data.Address2.Replace(',', ' ') + ", " + "Estado: " + data.State +  ", " + "Localizacion: " + data.LatLng.Replace(',', ' ') + ", " +
                              "Telefono: " + data.Phone + ", " + "Fecha de registro: " + data.RegisteredAt + ", " + "Fecha de nacimiento: " + data.Birth + ", " +
                              "Genero: " + data.Gender);

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