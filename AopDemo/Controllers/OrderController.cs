using System;
using System.Net;
using System.Threading.Tasks;
using AopDemo.Ordering.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AopDemo.Ordering.Api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public Task Create([FromBody] CreateOrderRequest request)
        {
            return _mediator.Send(request);
        }
    }
}