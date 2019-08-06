using AutoMapper;
using Limbs.Web.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Limbs.Web.Areas.Admin.Controllers
{
    public class EventsController : AdminBaseController
    {
        // GET: Admin/Events
        public async Task<ActionResult> Index()
        {
            var events = await Db.EventModels.Include(p => p.EventOrders).Include(p => p.EventType).ToListAsync();
            var eventsViewModel = Mapper.Map<List<EventViewModel>>(events);

            return View(eventsViewModel);
        }
    }
}