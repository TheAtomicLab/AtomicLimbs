using Limbs.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace Limbs.Web.Repositories
{
    public class ProductsRepository : IProductsRepository
    {

        private readonly ApplicationDbContext _context;

        public ProductsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductModel> Get()
        {
            return _context.ProductModels.ToList();
        }

        public ProductModel Get(int id)
        {
            return _context.ProductModels.Find(id);
        }

        public void Add(ProductModel entity)
        {
            _context.ProductModels.Add(entity);
            _context.SaveChanges();
        }

        public void Remove(ProductModel entity)
        {
            var obj = _context.ProductModels.Find(entity.Id);
            _context.ProductModels.Remove(obj);
            _context.SaveChanges();
        }
    }
}

