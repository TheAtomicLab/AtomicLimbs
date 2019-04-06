using System.Collections.Generic;
using System.Linq;
using Limbs.Web.Entities.DbContext;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Logic.Repositories
{
    public class UsersRepository : IUsersRepository
    {

        private readonly ApplicationDbContext _context;

        public UsersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<UserModel> Get()
        {
            return _context.UserModelsT.ToList();
        }

        public UserModel Get(int id)
        {
            return _context.UserModelsT.Find(id);
        }

        public void Add(UserModel entity)
        {
            _context.UserModelsT.Add(entity);
            _context.SaveChanges();
        }

        public void Remove(UserModel entity)
        {
            var obj = _context.UserModelsT.Find(entity.Id);
            _context.UserModelsT.Remove(obj);
            _context.SaveChanges();
        }
    }
}

