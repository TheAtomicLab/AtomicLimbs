using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Limbs.Web.Entities.DbContext;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Services
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

    public class EntityPageFilter
    {
        private int _index;
        private int _pageSize;
        public const int MaxPageSize = 24;

        public EntityPageFilter( int index = 0, int pageSize = MaxPageSize)
        {
            Index = index;
            PageSize = pageSize;
        }

        public int Index
        {
            get => _index < 0 ? 0 : _index;
            set => _index = value;
        }

        public int Skip => PageSize * Index;

        public int PageSize
        {
            get => _pageSize > MaxPageSize ? MaxPageSize : _pageSize;
            set => _pageSize = value;
        }
    }

    public class OrderFilters : EntityPageFilter
    {
        public OrderFilters()
        {
            ByAmputationType = false;
            ByStatus = false;
        }

        //esto se podria hacer con el type nullable y sin bool, pero romperia por todos lados
        [Display(Name = "Por estado:", Description = "")]
        public bool ByStatus { get; set; }
        public OrderStatus Status { get; set; }

        //esto se podria hacer con el type nullable y sin bool, pero romperia por todos lados
        [Display(Name = "Por tipo:", Description = "")]
        public bool ByAmputationType { get; set; }
        public AmputationType AmputationType { get; set; }

        [Display(Name = "Busqueda:", Description = "")]
        public string SearchTerm { get; set; }
    }
}