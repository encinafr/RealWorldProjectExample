using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.Services;
using MyShop.WebUI.Controllers;
using MyShop.WebUI.Tests.Mocks;

namespace MyShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class BasketControllerTest
    {
        [TestMethod]
        public void CanAddBasketItem()
        {
            IRepository<Basket> baskets = new Mocks.MockContext<Basket>();
            IRepository<Product> products = new Mocks.MockContext<Product>();
            IRepository<Order> order = new Mocks.MockContext<Order>();
            IRepository<Customer> customer = new Mocks.MockContext<Customer>();

            var httpContext = new MockHttpContext();

            IBasketSerivce basketSerivce = new BasketService(products, baskets);
            IOrderService orderService = new OrderService(order);

            var controller = new BasketController(basketSerivce, orderService , customer);

            //basketSerivce.AddToBasket(httpContext, "1");
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);
            controller.AddToBasket("1");

            Basket basket = baskets.Collection().FirstOrDefault();

            Assert.IsNotNull(basket);
            Assert.AreEqual(1, basket.BasketItems.Count());
            Assert.AreEqual("1", basket.BasketItems.ToList().FirstOrDefault().ProductId );
        }

        [TestMethod]
        public void CanGetSummaryViewModel()
        {
            IRepository<Basket> baskets = new Mocks.MockContext<Basket>();
            IRepository<Product> products = new Mocks.MockContext<Product>();
            IRepository<Order> order = new Mocks.MockContext<Order>();
            IRepository<Customer> customer = new Mocks.MockContext<Customer>();


            products.Insert(new Product() { Id = "1", Price = 10.00m });
            products.Insert(new Product() { Id = "2", Price = 2.00m });

            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 1 });
            baskets.Insert(basket);

            IBasketSerivce basketSerivce = new BasketService(products, baskets);
            IOrderService orderService = new OrderService(order);

            var controller = new BasketController(basketSerivce, orderService, customer);
            var httpContext = new MockHttpContext();

            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { Value = basket.Id });

            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            var result = controller.BasketSummary() as PartialViewResult;
            var basketSummary = (BasketSummaryViewModel)result.ViewData.Model;

            Assert.AreEqual(3, basketSummary.BasketCount);
            Assert.AreEqual(22.00m, basketSummary.BasketTotal);
        }

        [TestMethod]
        public void CanCheckoutAndCreateOrder()
        {
            IRepository<Product> productRespository = new Mocks.MockContext<Product>();
            IRepository<Customer> customer = new Mocks.MockContext<Customer>();

            productRespository.Insert(new Product()
            {
                Id = "1",
                Price = 10.00m
            });
            productRespository.Insert(new Product()
            {
                Id = "2",
                Price = 5.00m
            });

            IRepository<Basket> basketRepository = new Mocks.MockContext<Basket>();
            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem()
            {
                ProductId = "1",
                Quantity = 2,
                BasketId = basket.Id
            });
            basket.BasketItems.Add(new BasketItem()
            {
                ProductId = "2",
                Quantity = 1,
                BasketId = basket.Id
            });

            basketRepository.Insert(basket);

            IBasketSerivce basketSerivce = new BasketService(productRespository, basketRepository);
            IRepository<Order> orderRepository = new MockContext<Order>();
            IOrderService orderService = new OrderService(orderRepository);

            customer.Insert(new Customer()
            {
                Id = "1",
                Email = "pepe@gmail.com",
                ZipCode = "90201"
            });

            IPrincipal fakeUser = new GenericPrincipal(new GenericIdentity("pepe@gmail.com", "Forms"), null);

            var controller = new BasketController(basketSerivce, orderService, customer);
            var httpContext = new MockHttpContext();
            httpContext.User = fakeUser;
            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket")
            {
                Value = basket.Id
            });

            controller.ControllerContext = new ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            Order order = new Order();
            controller.Checkout(order);

            //Assert
            Assert.AreEqual(2, order.OrderItems.Count);
            Assert.AreEqual(0, basket.BasketItems.Count);

            Order orderInRep = orderRepository.Find(order.Id);

            Assert.AreEqual(2, orderInRep.OrderItems.Count);
        }
    }
}
