using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.WebUI.Common.BaseControllers;
using MyShopDataAccess.SQL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : ApiBaseController
    {
        IRepository<Customer> _customerRepository;
        IBasketSerivce _basketSerivce;
        IOrderService _orderService;

        public BasketController(IBasketSerivce basketSerivce, IOrderService orderService)
        {
            _basketSerivce = basketSerivce;
            _orderService = orderService;
            _customerRepository = _uow.Repository<SQLRepository<Customer>>();
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

        [Authorize]
        public ActionResult Checkout()
        {
            Customer customer = _customerRepository.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);

            if(customer != null)
            {
                Order order = new Order()
                {
                    Email = customer.Email,
                    City = customer.City,
                    State = customer.State,
                    Street = customer.Street,
                    FirstName = customer.FirstName,
                    Surname = customer.LastName,
                    ZipCode = customer.ZipCode
                };

                return View(order);
            }
            else
            {

                return RedirectToAction("Error");
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult Checkout(Order order)
        {
            var basketItems = _basketSerivce.GetBasketItems(this.HttpContext);
            order.OrderStatus = "Orser Created";
            order.Email = User.Identity.Name;
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