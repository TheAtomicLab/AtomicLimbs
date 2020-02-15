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
    [Authorize(Roles = AppRoles.Administrator)]
    public class SponsorsController : AdminBaseController
    {
        public async Task<ActionResult> Index(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sponsors = await Db.SponsorModels.Where(p => p.Event.Id == id)
                                                    .Include(p => p.Event)
                                                    .ToListAsync();

            if (sponsors == null)
                return RedirectToAction("Index", "Events");

            var eventModel = await Db.EventModels.Where(p => p.Id == id)
                                                    .FirstOrDefaultAsync();

            var sponsorsViewModel = Mapper.Map<List<SponsorViewModel>>(sponsors);            

            ViewBag.EventTitle = eventModel.Title;
            ViewBag.EventDescription = eventModel.Description;
            ViewData["EventId"] = id;

            return View(sponsorsViewModel);
        }

        public async Task<ActionResult> Edit(int EventId, int? SponsorId)
        {
            var sponsorViewModel = new SponsorViewModel();

            if(SponsorId != null)
            {
                var sponsor = await Db.SponsorModels.Where(p => p.Id == SponsorId)
                                                        .Include(p => p.Event)
                                                        .FirstOrDefaultAsync();

                if (sponsor != null)
                {
                    sponsorViewModel = Mapper.Map<SponsorViewModel>(sponsor);
                }
                    
            }

            return View(sponsorViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> AddUpdateSponsor(SponsorViewModel sponsorViewModel)
        {
            var sponsor = Db.SponsorModels.Where(p => p.Id == sponsorViewModel.Id)
                                          .Include(p => p.Event)
                                          .FirstOrDefault();

            var eventModel = Db.EventModels.Where(p => p.Id == sponsorViewModel.EventId)
                                           .FirstOrDefault();            

            sponsor = Mapper.Map(sponsorViewModel, sponsor);

            sponsor.Event = eventModel;

            if (sponsor.Id == 0)
                Db.SponsorModels.Add(sponsor);
           
            await Db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = sponsorViewModel.EventId });
        }

        public async Task<ActionResult> DeleteSponsor(int EventId, int? SponsorId)
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