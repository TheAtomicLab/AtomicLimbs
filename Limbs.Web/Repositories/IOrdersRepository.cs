﻿using Limbs.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace Limbs.Web.Repositories
{
    public interface IOrdersRepository : IRepository<OrderModel, int>
    {
        IEnumerable<OrderModel> GetByAssignedAmbassadorId(string id);
        void Update(OrderModel order);
    }

}
