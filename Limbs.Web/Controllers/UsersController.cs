using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Mvc;
using Limbs.Web.Models;
using Microsoft.AspNet.Identity;
using Limbs.Web.Helpers;
using Limbs.Web.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;



namespace Limbs.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Users
        public async Task<ActionResult> Index()
        {
            return View(await _db.UserModelsT.ToListAsync());
        }
        
        // GET: Users/Create
        [Authorize(Roles = "Unassigned")]
        public ActionResult Create()
        {
            return View();
        }
        
        // POST: Users/Create
        [HttpPost]
        [Authorize(Roles = "Unassigned")]
        public async Task<ActionResult> Create(UserModel userModel)
        {
            if (!ModelState.IsValid) return View("Create", userModel);
            
            //TODO (ale): refactor llamada al servicio
            var pointAddress = userModel.Country + ", " + userModel.City + ", " + userModel.Address;
            var point = Geolocalization.GetPointGoogle(pointAddress).Split(',');
            var lat = Convert.ToDouble(point[0].Replace('.',','));
            var lng = Convert.ToDouble(point[1].Replace('.', ','));

            userModel.Lat = lat;
            userModel.Long = lng;

            userModel.Email = User.Identity.GetUserName();
            userModel.UserId = User.Identity.GetUserId();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            await userManager.RemoveFromRoleAsync(userModel.UserId, "Unassigned");
            await userManager.AddToRoleAsync(userModel.UserId, "Requester");

            _db.UserModelsT.Add(userModel);
            await _db.SaveChangesAsync();

            return RedirectToAction("UserPanel");
        }
        
        [Authorize(Roles = "Requester")]
        public ActionResult UserPanel(string message)
        {
            var userId = User.Identity.GetUserId();
            var user = _db.UserModelsT.Single(c => c.UserId == userId);

            var orderList = _db.OrderModels.Where(c => c.OrderRequestor.UserId == userId).ToList();

            var lat = user.Lat;
            var lng = user.Long;
            //TODO (ale): porque se valida esto aca?
            var pointIsValid = Geolocalization.PointIsValid(lat, lng);

            var viewModel = new UserPanelViewModel
            {
                Order = orderList.ToList(),
                PointIsValid = pointIsValid,
                Message = message

            };

            return View(viewModel); 
     
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GetUserImage(string url)
        {
            var client = new HttpClient();

            var file = client.GetByteArrayAsync(url);

            return new FileContentResult(file.Result, "image/jpeg");
        }
    }

}
