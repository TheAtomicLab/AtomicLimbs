using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Limbs.Web.Entities.DbContext;
using Limbs.Web.Entities.Models;
using Limbs.Web.Entities.WebModels;

namespace Limbs.Web.Logic.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _db;

        public OrderService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<List<OrderModel>> GetPaged(OrderFilters filters)
        {
            var orders = _db.OrderModels
                .Include(c => c.OrderRequestor)
                .Include(c => c.OrderAmbassador)
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