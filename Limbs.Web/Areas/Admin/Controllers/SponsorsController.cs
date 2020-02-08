using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using Limbs.Web.Entities.Models;
using Limbs.Web.ViewModels.Admin;
using AutoMapper;

namespace Limbs.Web.Areas.Admin.Controllers
{
    [OverrideAuthorize(Roles = AppRoles.Administrator)]
    public class SponsorsController : AdminBaseController
    {
        // GET: Admin/Sponsors
        public async Task<ActionResult> Index(int eventId)
        {
            return View();
        }

        public async Task<ActionResult> GetSponsorsByEventId(int id)
        {
            var sponsors = await Db.SponsorModels.Where(p => p.Event.Id == id)
                                                    .Include(p => p.Event)
                                                    .ToListAsync();

            var ordersViewModel = Mapper.Map<List<SponsorViewModel>>(sponsors);

            return View(ordersViewModel);
        }

        public async Task<ActionResult> GetSponsorByEventId(int id)
        {
            var sponsor = await Db.SponsorModels.Where(p => p.Id == id)
                                                    .Include(p => p.Event)
                                                    .FirstOrDefaultAsync();

            var ordersViewModel = Mapper.Map<SponsorViewModel>(sponsor);

            return View(ordersViewModel);
        }

        //[HttpPost]
        public async Task<ActionResult> AddUpdateSponsor(SponsorViewModel sponsorViewModel)
        {
            var sponsor = Db.SponsorModels.Where(p => p.Id == sponsorViewModel.Id)
                                          .Include(p => p.Event)
                                          .FirstOrDefault();

            var eventModel = Db.EventModels.Where(p => p.Id == sponsorViewModel.EventId)
                                           .FirstOrDefault();

            if (sponsor != null)
            {
                sponsor.MobileImage = sponsorViewModel.MobileImage;
                sponsor.WebImage = sponsorViewModel.WebImage;

                sponsor.Event = eventModel;
            }
            else
            {
                sponsor = Mapper.Map<SponsorModel>(sponsorViewModel);

                sponsor.Event = eventModel;
                Db.SponsorModels.Add(sponsor);
            }                   

            await Db.SaveChangesAsync();

            return View();
        }

        public async Task<ActionResult> DeleteSponsor(int Id)
        {
            var sponsor = Db.SponsorModels.Where(p => p.Id == Id)
                                         .FirstOrDefault();

            Db.SponsorModels.Remove(sponsor);

            await Db.SaveChangesAsync();

            return View();
        }

    }
}