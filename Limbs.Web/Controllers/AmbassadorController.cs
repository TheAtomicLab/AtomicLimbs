using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Limbs.Web.Common.Geocoder;
using Limbs.Web.Common.Mail;
using Limbs.Web.Common.Mail.Entities;
using Limbs.Web.Entities.Models;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Limbs.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Limbs.Web.Controllers
{
    [DefaultAuthorize(Roles = AppRoles.Ambassador)]
    public class AmbassadorController : BaseController
    {
        private ApplicationUserManager _userManager;
        private readonly string _fromEmail = ConfigurationManager.AppSettings["Mail.From"];

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: /Ambassador/
        public ActionResult Index()
        {

            var currentUserId = User.Identity.GetUserId();

            IEnumerable<OrderModel> orderList = Db.OrderModels.Where(c => c.OrderAmbassador.UserId == currentUserId).Include(c => c.OrderRequestor).OrderByDescending(c => c.StatusLastUpdated).ToList();

            var deliveredOrdersCount = orderList.Count(c => c.Status == OrderStatus.Delivered);
            var pendingOrdersCount = orderList.Count(c => c.Status == OrderStatus.Pending || c.Status == OrderStatus.ArrangeDelivery || c.Status == OrderStatus.Ready);

            var pendingAssignationOrders = orderList.Where(o => o.Status == OrderStatus.PreAssigned).ToList();
            var pendingOrders = orderList.Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.ArrangeDelivery || o.Status == OrderStatus.Ready).ToList();
            var deliveredOrders = orderList.Where(o => o.Status == OrderStatus.Delivered).ToList();

            var viewModel = new ViewModels.AmbassadorPanelViewModel
            {
                PendingToAssignOrders = pendingAssignationOrders,
                PendingOrders = pendingOrders,
                DeliveredOrders = deliveredOrders,

                Stats = new ViewModels.OrderStats
                {
                    HandledOrders = deliveredOrdersCount,
                    PendingOrders = pendingOrdersCount
                }
            };


            return View(viewModel);
        }

        // GET: Ambassador/TermsAndConditions
        [OverrideAuthorize(Roles = AppRoles.Unassigned + "," + AppRoles.Ambassador)]
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        // GET: Ambassador/Create
        [OverrideAuthorize(Roles = AppRoles.Unassigned)]
        public ActionResult Create()
        {
            return View();
        }

        // GET: Ambassador/Edit
        [OverrideAuthorize(Roles = AppRoles.Ambassador + "," + AppRoles.Administrator)]
        public async Task<ActionResult> Edit(int? id)
        {
            AmbassadorModel ambassadorModel;
            var userId = User.Identity.GetUserId();
            if (!id.HasValue && User.IsInRole(AppRoles.Ambassador))
            {
                ambassadorModel = await Db.AmbassadorModels.FirstAsync(x => x.UserId == userId);
            }
            else
            {
                if (!id.HasValue) return HttpNotFound();
                ambassadorModel = await Db.AmbassadorModels.FindAsync(id);
            }
            if (ambassadorModel == null || !ambassadorModel.CanViewOrEdit(User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            ViewBag.ReturnUrl = Request.UrlReferrer?.AbsoluteUri;
            return View("Create", ambassadorModel);
        }

        // POST: Ambassador/Create
        [HttpPost]
        [OverrideAuthorize(Roles = AppRoles.Unassigned + "," + AppRoles.Ambassador + "," + AppRoles.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AmbassadorModel ambassadorModel, bool? termsAndConditions)
        {
            if (!User.IsInRole(AppRoles.Administrator))
            {
                ambassadorModel.Email = User.Identity.GetUserName();
                ambassadorModel.UserId = User.Identity.GetUserId();
            }

            await ValidateData(ambassadorModel, termsAndConditions);
            ViewBag.TermsAndConditions = termsAndConditions;
            if (!ModelState.IsValid) return View("Create", ambassadorModel);

            var ambassador = await Db.AmbassadorModels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == ambassadorModel.Id);
            if (ambassador == null)
            {
                //CREATE
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Db));
                await userManager.RemoveFromRoleAsync(ambassadorModel.UserId, AppRoles.Unassigned);
                await userManager.AddToRoleAsync(ambassadorModel.UserId, AppRoles.Ambassador);

                ambassadorModel.RegisteredAt = DateTime.UtcNow;

                Db.AmbassadorModels.Add(ambassadorModel);
                await Db.SaveChangesAsync();

                await SendWelcomeEmail(ambassadorModel);

                return RedirectToAction("Index");
            }

            if (!ambassadorModel.CanViewOrEdit(User)) return new HttpStatusCodeResult(HttpStatusCode.Conflict);

            if (!ambassador.CanViewOrEdit(User))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            //EDIT
            Db.Entry(ambassadorModel).State = EntityState.Modified;
            await Db.SaveChangesAsync();

            return RedirectToAction("Index", "Manage");
        }

        [HttpGet, OverrideAuthorize(Roles = AppRoles.Ambassador)]
        public async Task<ActionResult> Covid()
        {
            var currentUserId = User.Identity.GetUserId();
            var model = Db.COVIDEmbajadorEntregable.Where(p => p.Ambassador.UserId == currentUserId)
                                                    .Include(p => p.Ambassador)
                                                    .FirstOrDefault();

            if (model == null)
            {
                model = new COVIDEmbajadorEntregable
                {
                    CantEntregable = 0,
                    Ambassador = await Db.AmbassadorModels.FirstOrDefaultAsync(p => p.UserId == currentUserId),
                    TipoEntregable = 1
                };

                Db.COVIDEmbajadorEntregable.Add(model);
                await Db.SaveChangesAsync();
            }

            var vm = Mapper.Map<CovidEmbajadorEntregableViewModel>(model);

            DbGeography location = model.Ambassador.Location;
            vm.Orders =
                await (from covidOrg in Db.CovidOrganizationModels
                       join covidOrgAmb in Db.CovidOrgAmbassadorModels on covidOrg.Id equals covidOrgAmb.CovidOrgId into c
                       from c1 in c.DefaultIfEmpty()
                       join covidAmb in Db.COVIDEmbajadorEntregable on c1.CovidAmbassadorId equals covidAmb.Id into c2
                       from c3 in c2.DefaultIfEmpty()
                       join amb in Db.AmbassadorModels on c3.Id equals amb.Id into c4
                       from c5 in c4.DefaultIfEmpty()
                       group covidOrg by new { covidOrg.Id, Location = covidOrg.Location.Distance(location) ?? 0d } into g
                       orderby g.Key.Location
                       select new OrderCovidAmbassadorViewModel
                       {
                           OrgId = g.Key.Id,
                           OrdersInfo = g.Select(p => new OrderCovidInfoViewModel
                           {
                               CovidOrganization = p.CovidOrganization,
                               CovidOrganizationName = p.CovidOrganizationName,
                               Distance = p.Location.Distance(location) ?? 0d,
                               Quantity = p.Quantity,
                               AlreadySavedQuantity = p.CovidOrgAmbassadors.Any(x => x.CovidOrgId == p.Id && x.CovidAmbassadorId == model.Id),
                               QuantitySaved = p.CovidOrgAmbassadors.FirstOrDefault(x => x.CovidOrgId == p.Id && x.CovidAmbassadorId == model.Id).Quantity,
                               Ambassadors = p.CovidOrgAmbassadors.Select(x => new CovidAmbassador
                               {
                                   Name = x.CovidAmbassador.Ambassador.AmbassadorName,
                                   Lastname = x.CovidAmbassador.Ambassador.AmbassadorLastName,
                                   Quantity = x.Quantity
                               }).ToList(),
                               DeliveryDate = p.DeliveryDate
                           }).ToList()
                       }).ToListAsync();

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> GuardarCantidad(CovidEmbajadorEntregableViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Error = true
                });
            }

            var covidAmbassador = await Db.COVIDEmbajadorEntregable.FirstOrDefaultAsync(p => p.Id == model.Id);
            if (covidAmbassador == null)
            {
                return Json(new
                {
                    Error = true,
                    Msg = "El embajador no existe, recargue la página."
                });
            }

            covidAmbassador = Mapper.Map<CovidEmbajadorEntregableViewModel, COVIDEmbajadorEntregable>(model);
            covidAmbassador.Ambassador = await Db.AmbassadorModels.FirstOrDefaultAsync(p => p.Id == model.AmbassadorId);

            Db.COVIDEmbajadorEntregable.AddOrUpdate(covidAmbassador);

            await Db.SaveChangesAsync();

            return Json(new
            {
                Error = false
            });
        }

        [HttpPost]
        public async Task<ActionResult> SaveQuantityToOrder(CovidUpdateQuantity model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Error = true
                });
            }

            var covidOrgAmbassador = await Db.CovidOrgAmbassadorModels.FirstOrDefaultAsync(p => p.CovidAmbassadorId == model.CovidAmbassadorId && p.CovidOrgId == model.OrgId);
            if (covidOrgAmbassador == null)
            {
                covidOrgAmbassador = new CovidOrgAmbassador
                {
                    CovidAmbassadorId = model.CovidAmbassadorId,
                    CovidOrgId = model.OrgId,
                    Quantity = model.SavedQuantity
                };
            }

            var covidAmbassador = await Db.COVIDEmbajadorEntregable.Include(p => p.Ambassador).FirstOrDefaultAsync(p => p.Id == model.CovidAmbassadorId);
            covidAmbassador.CantEntregable -= covidOrgAmbassador.Quantity;

            Db.COVIDEmbajadorEntregable.AddOrUpdate(covidAmbassador);
            Db.CovidOrgAmbassadorModels.AddOrUpdate(covidOrgAmbassador);
            await Db.SaveChangesAsync();

            var covidOrg = await Db.CovidOrganizationModels.FirstOrDefaultAsync(p => p.Id == covidOrgAmbassador.CovidOrgId);

            var covidEmailInfo = new CovidSaveQuantityOrderEmail
            {
                Name = covidOrg.Name,
                Lastname = covidOrg.Surname,
                AmbassadorName = covidAmbassador.Ambassador.AmbassadorName,
                AmbassadorLastname = covidAmbassador.Ambassador.AmbassadorLastName,
                AmbassadorEmail = covidAmbassador.Ambassador.Email,
                AmbassadorAddress = $"{covidAmbassador.Ambassador.Address} ({covidAmbassador.Ambassador.Address2}), {covidAmbassador.Ambassador.City}, {covidAmbassador.Ambassador.State}, {covidAmbassador.Ambassador.Country}",
                AmbassadorPhoneNumber = covidAmbassador.Ambassador.Phone,
                Quantity = covidOrgAmbassador.Quantity
            };

            var mailMessage = new MailMessage
            {
                From = _fromEmail,
                To = covidOrg.Email,
                Subject = $"[Atomic Limbs] {covidEmailInfo.Quantity} Mascarillas en camino!",
                Body = CompiledTemplateEngine.Render("Mails.SaveQuantityOrderCovid", covidEmailInfo),
            };

            await AzureQueue.EnqueueAsync(mailMessage);

            return Json(new
            {
                Error = false
            });
        }

        private async Task ValidateData(AmbassadorModel ambassadorModel, bool? termsAndConditions)
        {
            ModelState[nameof(ambassadorModel.Id)]?.Errors.Clear();
            ModelState[nameof(ambassadorModel.UserId)]?.Errors.Clear();
            ModelState[nameof(ambassadorModel.Email)]?.Errors.Clear();

            ambassadorModel.Location = GeocoderLocation.GeneratePoint(ambassadorModel.LatLng.Split(','));
            /*
            ModelState[nameof(ambassadorModel.Location)]?.Errors.Clear();

            var pointAddress = ambassadorModel.Country + ", " + ambassadorModel.State + ", " + ambassadorModel.City + ", " + ambassadorModel.Address;
            var address = await GeocoderLocation.GetAddressAsync(pointAddress) as GoogleAddress;
            ambassadorModel.Location = GeocoderLocation.GeneratePoint(address);

            if (address == null)
                ModelState.AddModelError(nameof(ambassadorModel.Address), @"Dirección inválida.");
                
            if (address != null && address[GoogleAddressType.StreetNumber] == null)
                ModelState.AddModelError(nameof(ambassadorModel.Address), @"La dirección debe tener altura en la calle.");
            */

            if (ambassadorModel.Birth > DateTime.UtcNow.AddYears(-AmbassadorModel.MinYear))
                ModelState.AddModelError(nameof(ambassadorModel.Birth), $@"Debes ser mayor de {AmbassadorModel.MinYear} años.");

            if (termsAndConditions.HasValue && !termsAndConditions.Value)
                ModelState.AddModelError(nameof(termsAndConditions), @"Debe aceptar terminos y condiciones.");
        }

        private async Task SendWelcomeEmail(AmbassadorModel ambassadorModel)
        {
            var mailMessage = new MailMessage
            {
                From = ConfigurationManager.AppSettings["Mail.From"],
                To = ambassadorModel.Email,
                Body = CompiledTemplateEngine.Render("Mails.NewAmbassador", ambassadorModel),
                Subject = "¡Hola " + ambassadorModel.AmbassadorName + "! Ya sos un #EmbajadorAtómico del proyecto Limbs, leé esto para conocer los próximos pasos."
            };
            if (ambassadorModel.HasAlternativeEmail()) mailMessage.Cc = ambassadorModel.AlternativeEmail;

            await AzureQueue.EnqueueAsync(mailMessage);
        }
    }
}

