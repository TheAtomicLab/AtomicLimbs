using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using System.Net;
using Limbs.Web.Entities.Models;
using Limbs.Web.ViewModels.Admin;
using AutoMapper;

namespace Limbs.Web.Areas.Admin.Controllers
{
    [OverrideAuthorize(Roles = AppRoles.Administrator)]
    public class SponsorsController : AdminBaseController
    {
        public async Task<ActionResult> Index(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sponsors = await Db.SponsorModels.Where(p => p.Event.Id == id)
                                                    .Include(p => p.Event)
                                                    .ToListAsync();


            var sponsorsViewModel = Mapper.Map<List<SponsorViewModel>>(sponsors);

            if (sponsorsViewModel == null || sponsorsViewModel.Count == 0)
                return RedirectToAction("Index", "Events");

            ViewBag.EventTitle = sponsors[0].Event.Title;
            ViewBag.EventDescription = sponsors[0].Event.Description;

            return View(sponsorsViewModel);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sponsor = await Db.SponsorModels.Where(p => p.Id == id)
                                                    .Include(p => p.Event)
                                                    .FirstOrDefaultAsync();

            if(sponsor == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 

            var sponsorViewModel = Mapper.Map<SponsorViewModel>(sponsor);

            return View(sponsorViewModel);
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

        public async Task<ActionResult> DeleteSponsor(int? EventId, int? SponsorId)
        {
            if (SponsorId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sponsor = Db.SponsorModels.Where(p => p.Id == SponsorId)
                                         .FirstOrDefault();

            if (sponsor == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Db.SponsorModels.Remove(sponsor);

            await Db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = EventId });
        }

    }
}