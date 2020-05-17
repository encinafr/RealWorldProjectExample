using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderManagerController : Controller
    {
        IOrderService _orderService;

        public OrderManagerController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        // GET: OrderManager
        public ActionResult Index()
        {
            List<Order> orders = _orderService.GetOrdersList();
            return View(orders);
        }

        public ActionResult UpdateOrder(string Id)
        {
            ViewBag.StatusList = new List<string>()
            {
                "Order Created",
                "Payment Process",
                "order Shipper",
                "Order Complete"
            };
            Order order = _orderService.GetOrder(Id);
            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateOrder(Order updateOrder, string Id)
        {
            Order order = _orderService.GetOrder(Id);
            order.OrderStatus = updateOrder.OrderStatus;
            _orderService.UpdateOrder(order);

            return RedirectToAction("Index");
        }
    }
}