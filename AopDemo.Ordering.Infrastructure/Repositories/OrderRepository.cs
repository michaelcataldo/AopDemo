using System;
using System.Threading.Tasks;
using AopDemo.Ordering.Domain;

namespace AopDemo.Ordering.Infrastructure.Repositories
{
    public class OrderRepository : Repository, IOrderRepository
    {
        private readonly OrderingContext _dbContext;

        public OrderRepository(OrderingContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<Order> Get(int id)
        {
            return _dbContext.Orders.FindAsync(id);
        }

        public void Add(Order order)
        {
        }

        public void Update(Order order)
        {
        }

        public void Remove(Order order)
        {
        }
    }
}
