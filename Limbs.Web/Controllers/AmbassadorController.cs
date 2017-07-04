using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Limbs.Web.Models;
using Limbs.Web.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace Limbs.Web.Controllers
{
    [Authorize]
    public class AmbassadorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ambassador
        public async Task<ActionResult> Index()
        {
            return View(await db.AmbassadorModels.ToListAsync());
        }

        // GET: Ambassador/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmbassadorModel ambassadorModel = await db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }
            return View(ambassadorModel);
        }

        // GET: Ambassador/Create
        public ActionResult Create()
        {
            ViewBag.CountryList = GetCountryList();
            return View("View");
            // return View(new AmbassadorModel { Email = Ambassador.Identity.GetAmbassadorName(), Birth = DateTime.UtcNow.Date, Country = "Argentina"});
        }

        private static IEnumerable<SelectListItem> GetCountryList()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(x => new SelectListItem
                {
                    Text = new RegionInfo(x.LCID).DisplayName,
                    Value = new RegionInfo(x.LCID).DisplayName,
                }).OrderBy(x => x.Value).DistinctBy(x => x.Value);
        }

        // POST: Ambassador/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create(AmbassadorModel ambassadorModel)
        {
            ambassadorModel.Email = User.Identity.GetUserName();

            if (ModelState.IsValid)
            {

            //    ambassadorModel.Lat = 0;
            //    ambassadorModel.Long = 0;

                db.AmbassadorModels.Add(ambassadorModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CountryList = GetCountryList();

            return View("View", ambassadorModel);
        }

        public async Task<JsonResult> GetPoint(string address)
        {
            var httpClient = Api.GetHttpClient();
            var result = await httpClient.GetAsync(("Geocoder/").ToAbsoluteUri(new { address = address }));
            var value = await result.Content.ReadAsStringAsync();
            var r = JsonConvert.DeserializeObject<List<GeocoderResult>>(value);

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        // GET: Ambassador/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmbassadorModel ambassadorModel = await db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }
            return View(ambassadorModel);
        }

        // POST: Ambassador/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // public async Task<ActionResult> Edit([Bind(Include = "Id,Name,LastName,Email,Phone,Birth,Gender,Country,City,Address,Lat,Long")] AmbassadorModel ambassadorModel)
        public async Task<ActionResult> Edit([Bind(Include = "Id,AmbassadorName,Email,Birth,Gender,Address,Dni")] AmbassadorModel ambassadorModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ambassadorModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ambassadorModel);
        }

        // GET: Ambassador/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AmbassadorModel ambassadorModel = await db.AmbassadorModels.FindAsync(id);
            if (ambassadorModel == null)
            {
                return HttpNotFound();
            }
            return View(ambassadorModel);
        }

        // POST: Ambassador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AmbassadorModel ambassadorModel = await db.AmbassadorModels.FindAsync(id);
            db.AmbassadorModels.Remove(ambassadorModel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
