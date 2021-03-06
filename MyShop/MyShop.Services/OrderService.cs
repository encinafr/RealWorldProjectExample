﻿using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public class OrderService : IOrderService
    {
        IRepository<Order> _orderContext;

        public OrderService(IRepository<Order> orderContext)
        {
            _orderContext = orderContext;
        }

        public void CreateOrder(Order baseOrder, List<BasketItemViewModel> basketItem)
        {
            foreach (var item in basketItem)
            {
                baseOrder.OrderItems.Add(new OrderItem()
                {
                    ProductId = item.Id,
                    Image = item.ImageURL,
                    Price = item.Price,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity
                });
            }

            _orderContext.Insert(baseOrder);
            _orderContext.Commit();
        }

        public List<Order> GetOrdersList()
        {
          return _orderContext.Collection().ToList();
        }

        public Order GetOrder(string Id)
        {
            return _orderContext.Find(Id);
        }

        public void UpdateOrder(Order updateOrder)
        {
            _orderContext.Update(updateOrder);
            _orderContext.Commit();
        }
    }
}
