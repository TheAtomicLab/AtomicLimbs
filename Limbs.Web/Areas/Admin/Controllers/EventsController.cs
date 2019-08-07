using AutoMapper;
using Limbs.Web.Entities.Models;
using Limbs.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Limbs.Web.Areas.Admin.Controllers
{
    [OverrideAuthorize(Roles = AppRoles.Administrator)]

    public class EventsController : AdminBaseController
    {
        // GET: Admin/Events
        public async Task<ActionResult> Index()
        {
            var events = await Db.EventModels.Include(p => p.EventOrders).Include(p => p.EventType).ToListAsync();
            var eventsViewModel = Mapper.Map<List<EventViewModel>>(events);

            return View(eventsViewModel);
        }

        public async Task<ActionResult> GetOrders(int id)
        {
            var eventOrders = await Db.EventOrderModels.Where(p => p.EventId == id)
                                                        .Include(p => p.Order)
                                                        .Include(p => p.Order.OrderAmbassador)
                                                        .Include(p => p.Order.OrderRequestor)
                                                        .Include(p => p.Order.AmputationTypeFk).ToListAsync();

            var ordersViewModel = Mapper.Map<List<OrdersEventViewModel>>(eventOrders);

            return View(ordersViewModel);
        }

        public async Task<ActionResult> AssignEvent(int orderId)
        {
            var events = await Db.EventModels.Include(p => p.EventOrders).Include(p => p.EventType).ToListAsync();
            var eventsViewModel = Mapper.Map<List<AssignEventViewModel>>(events);

            foreach (var e in events)
            {
                foreach (var evm in eventsViewModel)
                {
                    if (e.Id != evm.Id) continue;

                    var getEvent = events.FirstOrDefault(p => p.Id == evm.Id);

                    evm.OrderId = orderId;
                    evm.EventOrderId = getEvent.EventOrders.FirstOrDefault(p => p.OrderId == orderId)?.Id;
                    evm.IsAssigned = getEvent.EventOrders.Any(p => p.OrderId == orderId);
                }
            }

            return View(eventsViewModel);
        }

        public async Task<ActionResult> RemoveFromEvent(int eventOrderId, int orderId)
        {
            var eventOrder = Db.EventOrderModels.FirstOrDefault(p => p.Id == eventOrderId);
            Db.EventOrderModels.Remove(eventOrder);

            var order = await Db.OrderModels.FirstOrDefaultAsync(p => p.Id == orderId);
            AmbassadorModel oldAmbassador = order.OrderAmbassador;

            order.OrderAmbassador = null;

            order.Status = OrderStatus.NotAssigned;
            order.StatusLastUpdated = DateTime.UtcNow;
            if (User != null)
                order.LogMessage(User, $"Remove ambassador {oldAmbassador.Email} to no-data");

            await Db.SaveChangesAsync();

            return RedirectToAction("AssignEvent", new { orderId });
        }

        public async Task<ActionResult> AddToEvent(int eventId, int orderId)
        {
            var newOrderEvent = new EventOrderModel
            {
                EventId = eventId,
                OrderId = orderId,
                Participated = false
            };

            Db.EventOrderModels.Add(newOrderEvent);

            var email = ConfigurationManager.AppSettings["ManotonAdmin"].ToString();
            var ambassador = await Db.AmbassadorModels.FirstOrDefaultAsync(p => p.Email == email);
            var order = await Db.OrderModels.FirstOrDefaultAsync(p => p.Id == orderId);
            AmbassadorModel oldAmbassador = order.OrderAmbassador;

            order.OrderAmbassador = ambassador;

            order.Status = OrderStatus.PreAssigned;
            order.StatusLastUpdated = DateTime.UtcNow;
            if (User != null)
                order.LogMessage(User, $"Change ambassador from {(oldAmbassador != null ? oldAmbassador.Email : "no-data")} to {ambassador.Email}");

            await Db.SaveChangesAsync();

            return RedirectToAction("AssignEvent", new { orderId });
        }
    }
}