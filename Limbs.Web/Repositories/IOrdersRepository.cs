using System.Collections.Generic;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Repositories
{
    public interface IOrdersRepository : IRepository<OrderModel, int>
    {
        IEnumerable<OrderModel> GetByAssignedAmbassadorId(string id);
        void Update(OrderModel order);
    }

}

