using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        IBasketSerivce _basketSerivce;
        IOrderService _orderService;

        public BasketController(IBasketSerivce basketSerivce, IOrderService orderService)
        {
            _basketSerivce = basketSerivce;
            _orderService = orderService;
        }
        // GET: Basket
        public ActionResult Index()
        {
            var model = _basketSerivce.GetBasketItems(this.HttpContext);
            return View(model);
        }

        public ActionResult AddToBasket(string Id)
        {
            _basketSerivce.AddToBasket(this.HttpContext, Id);
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromBasket(string Id)
        {
            _basketSerivce.RemoveBasket(this.HttpContext, Id);
            return RedirectToAction("Index");
        }

        public PartialViewResult BasketSummary()
        {
            var basketSummary = _basketSerivce.GetBasketSummary(this.HttpContext);
            return PartialView(basketSummary);
        }

        public ActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(Order order)
        {
            var basketItems = _basketSerivce.GetBasketItems(this.HttpContext);
            order.OrderStatus = "Orser Created";

            //process payment

            order.OrderStatus = "Payment Process";

            _orderService.CreateOrder(order, basketItems);
            _basketSerivce.ClearBasket(this.HttpContext);

            return RedirectToAction("Thankyou", new { OrderId = order.Id });
        }

        public ActionResult ThankYou(string orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}