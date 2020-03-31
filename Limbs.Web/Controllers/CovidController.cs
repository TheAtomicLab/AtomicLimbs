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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Limbs.Web.Controllers
{
    [AllowAnonymous]
    public class CovidController : BaseController
    {
        private readonly string _fromEmail = ConfigurationManager.AppSettings["Mail.From"];

        [HttpGet]
        public ActionResult Create()
        {
            return View(new CreateCovidOrganizationViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateCovidOrganizationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Error = true
                });
            }

            if (model.Quantity > 500)
            {
                return Json(new
                {
                    Error = true,
                    Msg = "La cantidad del pedido no debe superar las 500 unidades, por mas unidades, contactarse con Atomic Lab"
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

            var urlRedirect = Url.Action("Edit", "Covid", new { token = newCovidOrganization.Token }, Request.Url.Scheme);
            var covidEmailInfo = new CovidInfoEmail
            {
                FullName = $"{newCovidOrganization.Name} {newCovidOrganization.Surname}",
                Url = urlRedirect,
                Quantity = newCovidOrganization.Quantity
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
                UrlRedirect = urlRedirect,
                Error = false
            });
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return HttpNotFound();
            }

            var covidOrganizationModel = await Db.CovidOrganizationModels.FirstOrDefaultAsync(p => p.Token == token);

            if (covidOrganizationModel == null)
            {
                return HttpNotFound();
            }

            var covidOrganizationVm = Mapper.Map<EditCovidOrganizationViewModel>(covidOrganizationModel);
            return View(covidOrganizationVm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditCovidOrganizationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Error = true
                });
            }

            var covidOrganization = await Db.CovidOrganizationModels.FirstOrDefaultAsync(p => p.Token == model.Token);
            if (covidOrganization == null)
            {
                return Json(new
                {
                    Error = true,
                    Msg = "El pedido no existe, recargue la página."
                });
            }

            bool sendEmail = false;
            string previousEmail = string.Empty;

            if (model.Email != covidOrganization.Email)
            {
                var existingEmail = await Db.CovidOrganizationModels.FirstOrDefaultAsync(p => p.Email == model.Email && p.Id != model.Id);
                if (existingEmail != null)
                {
                    return Json(new
                    {
                        Error = true,
                        Msg = "El correo electronico ya se encuentra registrado"
                    });
                }
                else
                {
                    previousEmail = model.Email;
                    sendEmail = true;
                }
            }
            int quantityTmp = covidOrganization.Quantity;
            covidOrganization = Mapper.Map<CovidOrganizationModel>(model);
            covidOrganization.Quantity = quantityTmp;

            var sum = (await Db.CovidOrgAmbassadorModels
                           .GroupBy(o => o.CovidOrgId).Select(x => new
                           {
                               Id = x.Key,
                               Sum = x.Sum(z => z.Quantity)
                           }).FirstOrDefaultAsync(f => f.Id == covidOrganization.Id))?.Sum;

            var availableDiff = covidOrganization.Quantity - (sum ?? 0);
            bool hasDiff = false;

            if (model.Quantity < covidOrganization.Quantity && availableDiff < covidOrganization.Quantity)
            {
                hasDiff = availableDiff != 0;
                covidOrganization.Quantity -= availableDiff;
            }
            else
            {
                covidOrganization.Quantity = model.Quantity;
            }

            Db.CovidOrganizationModels.AddOrUpdate(covidOrganization);

            await Db.SaveChangesAsync();

            if (sendEmail)
            {
                var urlRedirect = Url.Action("Edit", "Covid", new { token = covidOrganization.Token }, Request.Url.Scheme);
                var covidEmailInfo = new CovidUpdateEmail
                {
                    FullName = $"{covidOrganization.Name} {covidOrganization.Surname}",
                    Url = urlRedirect,
                    PreviousEmail = previousEmail,
                    NewEmail = covidOrganization.Email
                };

                var mailMessage = new MailMessage
                {
                    From = _fromEmail,
                    To = covidOrganization.Email,
                    Subject = "[Atomic Limbs] Cambio de correo electrónico",
                    Body = CompiledTemplateEngine.Render("Mails.UpdateEmailOrderCovid", covidEmailInfo),
                };

                await AzureQueue.EnqueueAsync(mailMessage);
            }

            return Json(new
            {
                Error = false,
                Msg = !hasDiff ? null : $"La cantidad fue reducida a {covidOrganization.Quantity} ya que tiene máscarillas comprometidas por los embajadores"
            });
        }
    }
}

