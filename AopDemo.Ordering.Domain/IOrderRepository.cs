using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AopDemo.Ordering.Domain
{
    public interface IOrderRepository : IUnitOfWork
    {
        Task<Order> Get(int id);
        void Add(Order order);
        void Update(Order order);
        void Remove(Order order);
    }
}
