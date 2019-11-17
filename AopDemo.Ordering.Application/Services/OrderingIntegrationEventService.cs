using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AopDemo.Ordering.Application.Services
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        public Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            return Task.CompletedTask;
        }
    }
}
