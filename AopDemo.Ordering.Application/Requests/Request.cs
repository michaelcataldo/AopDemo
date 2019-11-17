using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace AopDemo.Ordering.Application.Requests
{
    public class Request
    {
        public Request()
        {
            RequestId = Guid.NewGuid();
        }

        public Guid RequestId { get; }
    }
}
