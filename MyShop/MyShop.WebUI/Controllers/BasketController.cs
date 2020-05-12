using MyShop.Core.Contracts;
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

        public BasketController(IBasketSerivce basketSerivce)
        {
            _basketSerivce = basketSerivce;
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
    }
}