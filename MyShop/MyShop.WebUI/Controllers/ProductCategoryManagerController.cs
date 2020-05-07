using MyShop.Core.Models;
using MyShop.DataAccess.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class ProductCategoryManagerController : Controller
    {
        InMemoryRepository<ProductCategory> _context;

        public ProductCategoryManagerController()
        {
            _context = new InMemoryRepository<ProductCategory>();
        }
            // GET: ProductManager
            public ActionResult Index()
        {
            List<ProductCategory> products = _context.Collection().ToList();

            return View(products);
        }

        public ActionResult Create()
        {
            ProductCategory product = new ProductCategory();
            return View(product);
        }

        [HttpPost]
        public ActionResult Create(ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(productCategory);
            }
            else
            {
                _context.Insert(productCategory);
                _context.Commit();

                return RedirectToAction("Index");
            }


        }

        public ActionResult Edit(string Id)
        {
            ProductCategory product = _context.Find(Id);

            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }
        }

        [HttpPost]
        public ActionResult Edit(ProductCategory product, string Id)
        {
            ProductCategory productToEdit = _context.Find(Id);

            if (productToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                    return View(product);

                productToEdit.Category = product.Category;

                _context.Commit();

                return RedirectToAction("Index");
            }

        }

        public ActionResult Delete(string Id)
        {
            ProductCategory productToDelete = _context.Find(Id);

            if (productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productToDelete);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            ProductCategory productToDelete = _context.Find(Id);

            if (productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                _context.Delete(Id);
                _context.Commit();

                return RedirectToAction("Index");
            }
        }
    }
}