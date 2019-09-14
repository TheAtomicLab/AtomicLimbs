using AutoMapper;
using Limbs.Web.ViewModels.Admin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class AmputationsController : AdminBaseController
    {
        // GET: Admin/Amputations
        public async Task<ActionResult> Index()
        {
            var amputations = await Db.AmputationTypeModels.ToListAsync();
            var amputationsViewModel = Mapper.Map<List<AmputationViewModel>>(amputations);

            return View(amputationsViewModel);
        }
    }
}