using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AopDemo.Ordering.Domain;
using MediatR;

namespace AopDemo.Ordering.Application.Requests
{
    public class CreateOrderRequestHandler : AsyncRequestHandler<CreateOrderRequest>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderRequestHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        protected override async Task Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var address = new Address(request.City, request.Street, request.State, request.Country, request.ZipCode);

            var order = new Order(
                request.UserId, 
                request.UserName, 
                request.CardNumber, 
                request.CardHolderName, 
                request.CardSecurityNumber,
                request.CardTypeId, 
                request.CardExpiration, 
                address);

            foreach (var orderItem in request.OrderItems)
            {
                order.AddOrderItem(orderItem.ProductId, orderItem.ProductName, orderItem.UnitPrice, orderItem.Units);
            }

            _orderRepository.Add(order);

            await _orderRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
