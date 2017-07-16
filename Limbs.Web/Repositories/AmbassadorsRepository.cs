using Limbs.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace Limbs.Web.Repositories
{
    public class AmbassadorsRepository : IAmbassadorsRepository
    {

        private readonly ApplicationDbContext _context;

        public AmbassadorsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<AmbassadorModel> Get()
        {
            return _context.AmbassadorModels.ToList();
        }

        public AmbassadorModel Get(int id)
        {
            return _context.AmbassadorModels.Find(id);
        }

        public void Add(AmbassadorModel entity)
        {
            _context.AmbassadorModels.Add(entity);
            _context.SaveChanges();
        }

        public void Remove(AmbassadorModel entity)
        {
            var obj = _context.AmbassadorModels.Find(entity.Id);
            _context.AmbassadorModels.Remove(obj);
            _context.SaveChanges();
        }
    }
}

