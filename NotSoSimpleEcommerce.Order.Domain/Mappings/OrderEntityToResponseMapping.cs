﻿using NotSoSimpleEcommerce.Order.Domain.Models;
using NotSoSimpleEcommerce.Shared.InOut.Responses;

namespace NotSoSimpleEcommerce.Order.Domain.Mappings
{
    public static class OrderEntityToResponseMapping
    {
        public static OrderResponse MapToResponse(this OrderEntity order)
        {
            return new OrderResponse(order.Id, order.ProductId, order.Quantity, order.BoughtBy, order.Status);
        }
    }
}