using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MediatR;

namespace AopDemo.Ordering.Application.Requests
{
    public class CreateOrderRequest : Request, IRequest
    {
        public CreateOrderRequest(
            IEnumerable<OrderItem> orderItems,
            string userId,
            string userName,
            string city,
            string street,
            string state,
            string country,
            string zipcode,
            string cardNumber,
            string cardHolderName,
            string cardSecurityNumber,
            int cardTypeId,
            DateTime cardExpiration)
        {
            OrderItems = orderItems;
            UserId = userId;
            UserName = userName;
            City = city;
            Street = street;
            State = state;
            Country = country;
            ZipCode = zipcode;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeId = cardTypeId;
            CardExpiration = cardExpiration;
        }

        [DataMember]
        public string UserId { get; private set; }

        [DataMember]
        public string UserName { get; private set; }

        [DataMember]
        public string City { get; private set; }

        [DataMember]
        public string Street { get; private set; }

        [DataMember]
        public string State { get; private set; }

        [DataMember]
        public string Country { get; private set; }

        [DataMember]
        public string ZipCode { get; private set; }

        [DataMember]
        public string CardNumber { get; private set; }

        [DataMember]
        public string CardHolderName { get; private set; }

        [DataMember]
        public DateTime CardExpiration { get; private set; }

        [DataMember]
        public string CardSecurityNumber { get; private set; }

        [DataMember]
        public int CardTypeId { get; private set; }

        [DataMember]
        public IEnumerable<OrderItem> OrderItems { get; private set; }

        public class OrderItem
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public decimal UnitPrice { get; set; }

            public decimal Discount { get; set; }

            public int Units { get; set; }
        }
    }
}
