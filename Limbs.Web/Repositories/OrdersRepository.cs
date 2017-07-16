using Limbs.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace Limbs.Web.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {

        private readonly ApplicationDbContext _context;

        public OrdersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<OrderModel> Get()
        {
            return _context.OrderModels.ToList();
        }

        public OrderModel Get(int id)
        {
            return _context.OrderModels.Find(id);
        }

        public void Add(OrderModel entity)
        {
            _context.OrderModels.Add(entity);
            _context.SaveChanges();
        }

        public void Remove(OrderModel entity)
        {
            var obj = _context.OrderModels.Find(entity.Id);
            _context.OrderModels.Remove(obj);
            _context.SaveChanges();
        }
    }
}

