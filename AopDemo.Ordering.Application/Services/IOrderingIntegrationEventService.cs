using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AopDemo.Ordering.Application.Services
{
    public interface IOrderingIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
    }
}
