using System.Web;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using System.Net;
using Limbs.Web.Entities.Models;
using Limbs.Web.ViewModels.Admin;
using Limbs.Web.Logic.Repositories.Interfaces;
using Limbs.Web.Common.Extensions;
using AutoMapper;

namespace Limbs.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppRoles.Administrator)]
    public class SponsorsController : AdminBaseController
    {
        private readonly IUserFiles _userFiles;

        public SponsorsController(IUserFiles userFiles)
        {
            _userFiles = userFiles;
        }

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
            SponsorViewModel sponsorViewModel;

            var sponsor = await Db.SponsorModels.Where(p => p.Id == SponsorId)
                                                       .Include(p => p.Event)
                                                       .FirstOrDefaultAsync();
            if (sponsor == null)
                sponsorViewModel = new SponsorViewModel { EventId = EventId };
            else
                sponsorViewModel = Mapper.Map<SponsorViewModel>(sponsor);

            return View(sponsorViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(SponsorViewModel sponsorViewModel)
        {
            var sponsor = Db.SponsorModels.Where(p => p.Id == sponsorViewModel.Id)
                                          .Include(p => p.Event)
                                          .FirstOrDefault();

            var eventModel = Db.EventModels.Where(p => p.Id == sponsorViewModel.EventId)
                                           .FirstOrDefault();            

            sponsor = Mapper.Map(sponsorViewModel, sponsor);

            sponsor.Event = eventModel;

            if (Request.Files != null && Request.Files.Count > 0)
            {
                foreach (string fileStr in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileStr];

                    if (file.ContentLength == 0 || file.ContentLength > 1000000 * 5 || !file.IsImage())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ocurrio un error en la imagen");

                    //var fileName = Guid.NewGuid().ToString("N") + ".jpg";
                    //var fileUrl = _userFiles.UploadOrderFile(file.InputStream, fileName);

                    //if (file.FileName == "webImg")
                    //    sponsor.WebImage = fileUrl.ToString();
                    //else if (file.FileName == "MobileImg")
                    //    sponsor.MobileImage = fileUrl.ToString();
                }
            }

            if (sponsor.Id == 0)
                Db.SponsorModels.Add(sponsor);
           
           // await Db.SaveChangesAsync();

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

            var webImgNameBlob = sponsor.WebImage.Split('/').LastOrDefault();
            var mobileImgNameBlob = sponsor.MobileImage.Split('/').LastOrDefault();

            await _userFiles.RemoveImageAsync(webImgNameBlob);
            await _userFiles.RemoveImageAsync(mobileImgNameBlob);

            Db.SponsorModels.Remove(sponsor);

            await Db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = EventId });
        }

    }
}