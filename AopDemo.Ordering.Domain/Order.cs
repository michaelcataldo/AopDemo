using System;
using System.Collections.Generic;
using System.Text;

namespace AopDemo.Ordering.Domain
{
    public class Order : Entity
    {
        public Order(
            string userId,
            string userName,
            string cardNumber,
            string cardHolderName,
            string cardSecurityNumber,
            int cardTypeId,
            DateTime cardExpiration,
            Address address)
        {
            
        }

        public void AddOrderItem(int productId, string productName, decimal unitPrice, int units)
        {

        }
    }
}
