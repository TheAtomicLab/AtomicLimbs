using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Xml.Linq;
using Limbs.Web.Entities.DbContext;
using Limbs.Web.Entities.Models;
using Limbs.Web.Entities.WebModels;
using Limbs.Web.Logic.Helpers;

namespace Limbs.Web.Logic.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _db;
        public OrderService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<AmbassadorModel> GetAmbassadorAutoAssignAsync(OrderModel order)
        {   
            AmbassadorModel closestAmbassador = null;

            DbGeography location = order.OrderRequestor.Location;
            closestAmbassador = await _db.AmbassadorModels.Include(p => p.User).Include(p => p.RefusedOrders)
                                                    .OrderBy(p => p.Location.Distance(location))
                                                    .FirstOrDefaultAsync(p => (p.Location.Distance(location) / 1000) <= 50d &&
                                                        p.User.EmailConfirmed &&
                                                        !p.RefusedOrders.Any(x => x.OrderId == order.Id) &&
                                                        !p.OrderModel.Any(o =>
                                                            o.Status == OrderStatus.PreAssigned ||
                                                            o.Status == OrderStatus.Pending ||
                                                            o.Status == OrderStatus.Ready ||
                                                            o.Status == OrderStatus.ArrangeDelivery));

            if (closestAmbassador == null)
            {
                HttpResponseMessage response = await CallAndreaniApiAsync();
                if (!response.IsSuccessStatusCode)
                    return closestAmbassador;

                string content = await response.Content.ReadAsStringAsync();
                XDocument createXml = XDocument.Parse(content);

                IEnumerable<UbicacionAndreani> ubicaciones = createXml.Descendants("item").Select(p => new UbicacionAndreani
                {
                    Descripcion = p.Element("Descripcion").Value,
                    Direccion = p.Element("Direccion").Value,
                    HoradeTrabajo = p.Element("HoradeTrabajo").Value,
                    Latitud = p.Element("Latitud").Value,
                    Longitud = p.Element("Longitud").Value,
                    Mail = p.Element("Mail").Value,
                    Numero = p.Element("Numero").Value,
                    Responsable = p.Element("Responsable").Value,
                    Resumen = p.Element("Resumen").Value,
                    Sucursal = p.Element("Sucursal").Value,
                    Telefono1 = p.Element("Telefono1").Value,
                    Telefono2 = p.Element("Telefono2").Value,
                    Telefono3 = p.Element("Telefono3").Value,
                    TipoSucursal = p.Element("TipoSucursal").Value,
                    TipoTelefono1 = p.Element("TipoTelefono1").Value,
                    TipoTelefono2 = p.Element("TipoTelefono2").Value,
                    TipoTelefono3 = p.Element("TipoTelefono3").Value,
                    Location = SiteHelper.GeneratePoint($"{p.Element("Latitud").Value},{p.Element("Longitud").Value}".Split(','))
                });

                var listAmbassadors = await _db.AmbassadorModels
                                            .Include(p => p.User)
                                            .Include(p => p.RefusedOrders)
                                            .Where(p => p.User.EmailConfirmed &&
                                                        !p.RefusedOrders.Any(x => x.OrderId == order.Id) &&
                                                        !p.OrderModel.Any(o => o.Status == OrderStatus.PreAssigned ||
                                                            o.Status == OrderStatus.Pending ||
                                                            o.Status == OrderStatus.Ready ||
                                                            o.Status == OrderStatus.ArrangeDelivery))
                                            .OrderBy(p => p.Location.Distance(p.Location))
                                            .ToListAsync();

                closestAmbassador = listAmbassadors.FirstOrDefault(p =>
                                                        ubicaciones.Any(x => (x.Location.Distance(p.Location) / 1000) <= 20d));
            }

            return closestAmbassador;
        }

        public async Task<HttpResponseMessage> CallAndreaniApiAsync()
        {
            string soapString = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                    <env:Envelope
                                        xmlns:env=""http://www.w3.org/2003/05/soap-envelope""
                                        xmlns:ns1=""urn:ConsultarSucursales""
                                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                                        xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                                        xmlns:ns2=""http://xml.apache.org/xml-soap""
                                        xmlns:ns3=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""
                                        xmlns:enc=""http://www.w3.org/2003/05/soap-encoding"">
                                        <env:Header>
                                            <ns3:Security env:mustUnderstand=""true"">
                                                <ns3:UsernameToken>
                                                    <ns3:Username></ns3:Username>
                                                    <ns3:Password></ns3:Password>
                                                </ns3:UsernameToken>
                                            </ns3:Security>
                                        </env:Header>
                                        <env:Body>
                                            <ns1:ConsultarSucursales env:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
                                                <Consulta xsi:type=""ns2:Map"">
                                                    <item>
                                                        <key xsi:type=""xsd:string"">consulta</key>
                                                        <value xsi:type=""ns2:Map"">
                                                            <item>
                                                                <key xsi:type=""xsd:string"">Localidad</key>
                                                                <value xsi:type=""xsd:string""></value>
                                                            </item>
                                                            <item>
                                                                <key xsi:type=""xsd:string"">CodigoPostal</key>
                                                                <value xsi:type=""xsd:string""></value>
                                                            </item>
                                                            <item>
                                                                <key xsi:type=""xsd:string"">Provincia</key>
                                                                <value xsi:type=""xsd:string""></value>
                                                            </item>
                                                        </value>
                                                    </item>
                                                </Consulta>
                                            </ns1:ConsultarSucursales>
                                        </env:Body>
                                    </env:Envelope>";

            HttpResponseMessage response = await SiteHelper.PostXmlRequestAsync("https://sucursales.andreani.com/ws", soapString, "ConsultarSucursales");
            return response;
        }

        public async Task<bool> AssignmentAmbassadorAsync(int id, int idOrder, IPrincipal user, IOrderNotificationService _ns)
        {
            OrderModel order = await _db.OrderModels.Include(x => x.OrderAmbassador).Include(x => x.RenderPieces).Include(x => x.OrderRequestor).FirstOrDefaultAsync(x => x.Id == idOrder);

            if (order == null)
                return false;

            AmbassadorModel newAmbassador = await _db.AmbassadorModels.FindAsync(id);

            if (newAmbassador == null)
                return false;

            AmbassadorModel oldAmbassador = order.OrderAmbassador;
            OrderStatus orderOldStatus = order.Status;

            order.OrderAmbassador = newAmbassador;
            order.Status = OrderStatus.PreAssigned;
            order.StatusLastUpdated = DateTime.UtcNow;
            order.RenderPieces.ForEach(p => p.Printed = false);
            if (user != null)
                order.LogMessage(user, $"Change ambassador from {(oldAmbassador != null ? oldAmbassador.Email : "no-data")} to {newAmbassador.Email}");

            await _db.SaveChangesAsync();

            await _ns.SendStatusChangeNotification(order, orderOldStatus, OrderStatus.PreAssigned);
            await _ns.SendAmbassadorChangedNotification(order, oldAmbassador, newAmbassador);

            return true;
        }

        public Task<List<OrderModel>> GetPaged(OrderFilters filters)
        {
            var orders = _db.OrderModels
                .Include(p => p.AmputationTypeFk)
                .Include(c => c.OrderRequestor)
                .Include(c => c.OrderAmbassador)
                .Include(p => p.RenderPieces)
                .OrderByDescending(x => x.Date);
            
            var ordersFiltered = orders
                .Where(x => 
                    (!filters.ByStatus || x.Status == filters.Status)
                    &&
                    (!filters.ByAmputationType || x.AmputationType == filters.AmputationType)
                    &&
                    (filters.SearchTerm == null || filters.SearchTerm == "" ||
                           x.OrderRequestor.Email.Contains(filters.SearchTerm) &&
                           x.OrderRequestor.AlternativeEmail.Contains(filters.SearchTerm) &&
                           x.OrderRequestor.Country.Contains(filters.SearchTerm) &&
                           x.OrderAmbassador.Email.Contains(filters.SearchTerm) &&
                           x.OrderRequestor.AlternativeEmail.Contains(filters.SearchTerm) &&
                           x.OrderRequestor.Country.Contains(filters.SearchTerm)));

            return ordersFiltered.ToListAsync();
        }
    }
}