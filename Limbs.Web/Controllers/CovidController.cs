using AutoMapper;
using Limbs.Web.Common.Mail;
using Limbs.Web.Common.Mail.Entities;
using Limbs.Web.Entities.Models;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Limbs.Web.ViewModels;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Limbs.Web.Controllers
{
    [AllowAnonymous, RoutePrefix("~/covid")]
    public class CovidController : BaseController
    {
        private readonly string _fromEmail = ConfigurationManager.AppSettings["Mail.From"];

        [HttpGet, Route]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, Route, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateCovidOrganizationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Error = true
                });
            }

            var existingEmail = await Db.CovidOrganizationModels.FirstOrDefaultAsync(p => p.Email == model.Email);
            if (existingEmail != null)
            {
                return Json(new
                {
                    Error = true,
                    Msg = "El correo electronico ya se encuentra registrado"
                });
            }

            var newCovidOrganization = Mapper.Map<CovidOrganizationModel>(model);
            Db.CovidOrganizationModels.Add(newCovidOrganization);
            await Db.SaveChangesAsync();

            var id = newCovidOrganization.Id;

            var currentTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            byte[] key = Encoding.UTF8.GetBytes($"{Guid.NewGuid():N}-{id}-{currentTimeStamp}");

            newCovidOrganization.Token = Convert.ToBase64String(key);
            Db.CovidOrganizationModels.AddOrUpdate(newCovidOrganization);

            await Db.SaveChangesAsync();

            var covidEmailInfo = new CovidInfoEmail
            {
                FullName = $"{newCovidOrganization.Name} {newCovidOrganization.Surname}",
                Url = Url.Action("Edit", "Covid", new { token = newCovidOrganization.Token }, Request.Url.Scheme)
            };

            var mailMessage = new MailMessage
            {
                From = _fromEmail,
                To = newCovidOrganization.Email,
                Subject = "[Atomic Limbs] Tu pedido de mascarillas fue realizado con éxito",
                Body = CompiledTemplateEngine.Render("Mails.NewOrderCovid", covidEmailInfo),
            };

            await AzureQueue.EnqueueAsync(mailMessage);

            return Json(new
            {
                NoGenerateToken = false,
                EmailSended = true,
                Error = false
            });
        }
    }
}

