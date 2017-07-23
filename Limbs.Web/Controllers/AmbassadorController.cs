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
using System.Web.Script.Serialization;
using System.Web.Http;


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
             var lat = GetLatGoogle(ambassadorModel.Address);
                ambassadorModel.Lat = lat;
                //ambassadorModel.Lat = 40.731;
                var lng = GetLongGoogle(ambassadorModel.Address);
                ambassadorModel.Long = lng;

                //ambassadorModel.Long = 40.731;


                db.AmbassadorModels.Add(ambassadorModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CountryList = GetCountryList();

            return View("View", ambassadorModel);
        }
        
        public ActionResult GetPointGoogle(String Address)
        {
            var address = String.Format("http://maps.google.com/maps/api/geocode/json?address={0}&sensor=false", Address.Replace(" ", "+"));
            var result = new System.Net.WebClient().DownloadString(address);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(result);

            var lat = dict["results"][0]["geometry"]["location"]["lat"];
            var lng = dict["results"][0]["geometry"]["location"]["lng"];

            var latlng = Convert.ToString(lat).Replace(',','.') + ',' + Convert.ToString(lng).Replace(',', '.');
            return Json(new { result = latlng }, JsonRequestBehavior.AllowGet);
            //return Convert.ToDouble(latlng);
            // return jss.Deserialize<dynamic>(result);
        }

        // POST: Ambassador/GetLatGoogle
        [HttpPost]
      public double GetLatGoogle(String Address)
      {
          var address = String.Format("http://maps.google.com/maps/api/geocode/json?address={0}&sensor=false", Address.Replace(" ", "+"));
          var result = new System.Net.WebClient().DownloadString(address);
          JavaScriptSerializer jss = new JavaScriptSerializer();
          var dict = jss.Deserialize<dynamic>(result);

          var lat = dict["results"][0]["geometry"]["location"]["lat"];

          return Convert.ToDouble(lat);
          // return jss.Deserialize<dynamic>(result);
      }

      public double GetLongGoogle(String Address)
      {
          var address = String.Format("http://maps.google.com/maps/api/geocode/json?address={0}&sensor=false", Address.Replace(" ", "+"));
          var result = new System.Net.WebClient().DownloadString(address);
          JavaScriptSerializer jss = new JavaScriptSerializer();
          var dict = jss.Deserialize<dynamic>(result);

          var lng = dict["results"][0]["geometry"]["location"]["lng"];

          return Convert.ToDouble(lng);
          // return jss.Deserialize<dynamic>(result);
      }

   /*   public string GetPointGoogle(string address)
      {
        var apiGoogle = "https://maps.googleapis.com/maps/api/geocode/json?address=" + Server.UrlEncode(address) + "&key=AIzaSyBwDPOhcUy7GhHc4RhteO1vVxpgo7ynl6Q";

          StreamReader sr = new StreamReader(apiGoogle);


          var jss = new JavaScriptSerializer();
          var dict = jss.Deserialize<dynamic>(apiGoogle);

          var lat = dict["results"][0]["geometry"]["location"]["lat"];
          var lng = dict["results"][0]["geometry"]["location"]["lng"];
          return lat + lng;
          string sContentsa = sr.ReadToEnd();
          sr.Close();
      }
      */
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
        /*       --Leave comments for possible evolution-#idEvolution = 1#--lucaslopezf--##
                 
        public async Task<ActionResult> Edit([Bind(Include = "Id,AmbassadorName,Email,Birth,Gender,Address,Dni,AtributoEmbajador1,AtributoEmbajador2,AtributoEmbajador3")] AmbassadorModel ambassadorModel)
        */
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
