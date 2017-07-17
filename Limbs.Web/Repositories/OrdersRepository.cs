﻿using Limbs.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;


namespace Limbs.Web.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {


        protected DbSet<OrderModel> GetDbSet()
        {
            return _context.Set<OrderModel>();
        }

        private readonly ApplicationDbContext _context;

        public OrdersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<OrderModel> Get()
        {
            return _context.OrderModels.ToList();
        }

        public IEnumerable<OrderModel> GetByAssignedAmbassadorId(string id)
        {
            return _context.OrderModels.Where(m => m.OrderUser.Id == id).ToList();
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

        public void Update(OrderModel order)
        {
            GetDbSet().Attach(order);
            _context.Entry(order).State = EntityState.Modified;
        }

    }
}

