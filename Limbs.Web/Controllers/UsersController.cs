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
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public async Task<ActionResult> Index()
        {
            return View(await db.UserModelsT.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModel userModel = await db.UserModelsT.FindAsync(id);
            if (userModel == null)
            {
                return HttpNotFound();
            }
            return View(userModel);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.CountryList = GetCountryList();
            return View("View");
           // return View(new UserModel { Email = User.Identity.GetUserName(), Birth = DateTime.UtcNow.Date, Country = "Argentina"});
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

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create(UserModel userModel)
        {
            userModel.Email = User.Identity.GetUserName();

            if (ModelState.IsValid)
            {

                userModel.Lat = 0;
                userModel.Long = 0;

                db.UserModelsT.Add(userModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CountryList = GetCountryList();

            return View("View", userModel);
        }

        public async Task<JsonResult> GetPoint(string address)
        {
            var httpClient = Api.GetHttpClient();
            var result = await httpClient.GetAsync(("Geocoder/").ToAbsoluteUri(new { address = address }));
            var value = await result.Content.ReadAsStringAsync();
            var r = JsonConvert.DeserializeObject<List<GeocoderResult>>(value);

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModel userModel = await db.UserModelsT.FindAsync(id);
            if (userModel == null)
            {
                return HttpNotFound();
            }
            return View(userModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ResponsableName,UserName,Email,Dni,Phone,Birth,Gender,Address,ProthesisType,ProductType,AmputationType,Lat,Long")] UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(userModel);
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModel userModel = await db.UserModelsT.FindAsync(id);
            if (userModel == null)
            {
                return HttpNotFound();
            }
            return View(userModel);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UserModel userModel = await db.UserModelsT.FindAsync(id);
            db.UserModelsT.Remove(userModel);
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

    public class GeocoderResult
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Nombre { get; set; }
    }
}
