using AutoMapper;
using Limbs.Web.Common.Extensions;
using Limbs.Web.Entities.Models;
using Limbs.Web.Logic.Repositories.Interfaces;
using Limbs.Web.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Limbs.Web.Areas.Admin.Controllers
{
    [OverrideAuthorize(Roles = AppRoles.Administrator)]
    public class AmputationsController : AdminBaseController
    {
        private readonly IUserFiles _userFiles;

        public AmputationsController(IUserFiles userFiles)
        {
            _userFiles = userFiles;
        }

        // GET: Admin/Amputations
        public async Task<ActionResult> Index()
        {
            var amputations = await Db.AmputationTypeModels.ToListAsync();
            var amputationsViewModel = Mapper.Map<List<AmputationViewModel>>(amputations);

            return View(amputationsViewModel);
        }

        // GET: Admin/Amputations/Id
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var amputation = await Db.AmputationTypeModels.FirstOrDefaultAsync(p => p.Id == id);
            if (amputation == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var amputationsViewModel = Mapper.Map<EditAmputationViewModel>(amputation);

            return View(amputationsViewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditAmputationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var amputation = await Db.AmputationTypeModels.FirstOrDefaultAsync(p => p.Id == model.Id);
            if (amputation == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            amputation = Mapper.Map(model, amputation);

            if (Request.Files != null && Request.Files.Count > 0)
            {
                foreach (string fileStr in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileStr];

                    if (file.ContentLength == 0 || file.ContentLength > 1000000 * 5 || !file.IsImage())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ocurrio un error en la imagen");

                    var fileName = Guid.NewGuid().ToString("N") + ".jpg";
                    var fileUrl = _userFiles.UploadOrderFile(file.InputStream, "amputationType/" + fileName, "site");

                    amputation.PrimaryUrlImage = fileUrl.ToString();
                }

                await _userFiles.RemoveImageAsync(model.UrlImage, "site");
            }

            Db.AmputationTypeModels.AddOrUpdate(amputation);
            await Db.SaveChangesAsync();

            var urlAction = Url.Action("Index", "Amputations", new { id = amputation.Id, area = "Admin" });

            return Json(new
            {
                Action = urlAction
            });
        }
    }
}