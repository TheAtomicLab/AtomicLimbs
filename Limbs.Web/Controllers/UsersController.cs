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
using Microsoft.AspNet.Identity.EntityFramework;



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
        [Authorize(Roles = "Unassigned")]
        public ActionResult Create()
        {
            //ViewBag.CountryList = GetCountryList();
            return View("Create");
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
        [Authorize(Roles = "Unassigned")]
        public async Task<ActionResult> Create(UserModel userModel)
        {
            userModel.Email = User.Identity.GetUserName();
            userModel.UserId = User.Identity.GetUserId();
            // userModel.OrderModelId = 0;

            if (ModelState.IsValid)
            {
                //   userModel.OrderModelId = new List<int>();
                //   userModel.OrderModel = new List<OrderModel>();
                var pointAddress = userModel.Country + ", " + userModel.City + ", " + userModel.Address;
                
                var point = Helpers.Geolocalization.GetPointGoogle(pointAddress).Split(',');
                var lat = Convert.ToDouble(point[0].Replace('.',','));
                var lng = Convert.ToDouble(point[1].Replace('.', ','));
                userModel.Lat = lat;
                userModel.Long = lng;

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                await userManager.RemoveFromRoleAsync(userModel.UserId, "Unassigned");
                await userManager.AddToRoleAsync(userModel.UserId, "Requester");


                db.UserModelsT.Add(userModel);
                await db.SaveChangesAsync();
                return RedirectToAction("UserPanel");
            }

           // ViewBag.CountryList = GetCountryList();

            return View("Create", userModel);
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
      
        /*
        public async Task<ActionResult> UserPanel(int? id)
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
            // return View();
        }
        */

        [Authorize(Roles = "Requester")]
        public ActionResult UserPanel(string message)
        {
            var userId = User.Identity.GetUserId();
            UserModel user = db.UserModelsT.Single(c => c.UserId == userId);

            IEnumerable<OrderModel> orderList = db.OrderModels.Where(c => c.OrderRequestor.UserId == userId).ToList();

            var lat = user.Lat;
            var lng = user.Long;

            var pointIsValid = Helpers.Geolocalization.PointIsValid(lat, lng);

            var viewModel = new ViewModels.UserPanelViewModel()
            {
                Order = orderList.ToList(),
                PointIsValid = pointIsValid,
                Message = message

            };

            return View(viewModel); 
     
        }

        public ActionResult LoginUser(string email)
        {
            UserModel userModel = db.UserModelsT.First(u => u.Email == email) ;

            //return View("UserPanel" + "/" + userModel.Id);
            return RedirectToAction("UserPanel",new { userModel.Id });
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
