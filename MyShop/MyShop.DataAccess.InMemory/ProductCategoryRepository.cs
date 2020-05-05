using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.InMemory
{
    public class ProductCategoryRepository
    {
        ObjectCache cache = MemoryCache.Default;
        List<ProductCategory> ProductCategories = new List<ProductCategory>();

        public ProductCategoryRepository()
        {
            ProductCategories = cache["ProductCategories"] as List<ProductCategory>;

            if (ProductCategories == null)
            {
                ProductCategories = new List<ProductCategory>();
            }
        }

        public void Commit()
        {
            cache["ProductCategories"] = ProductCategories;
        }

        public void Insert(ProductCategory p)
        {
            ProductCategories.Add(p);
        }

        public void Update(ProductCategory ProductCategory)
        {
            ProductCategory ProductCategoryToUpdate = ProductCategories.Find(p => p.Id == ProductCategory.Id);

            if (ProductCategoryToUpdate != null)
            {
                ProductCategoryToUpdate = ProductCategory;
            }
            else
            {
                throw new Exception("ProductCategory no found");
            }

        }

        public ProductCategory Find(string id)
        {
            ProductCategory ProductCategory = ProductCategories.Find(p => p.Id == id);

            if (ProductCategory != null)
            {
                return ProductCategory;
            }
            else
            {
                throw new Exception("ProductCategory no found");
            }

        }

        public IQueryable<ProductCategory> Collection()
        {
            return ProductCategories.AsQueryable();
        }

        public void Delete(string id)
        {
            ProductCategory ProductCategoryToDelete = ProductCategories.Find(p => p.Id == id);

            if (ProductCategoryToDelete != null)
            {
                ProductCategories.Remove(ProductCategoryToDelete);
            }
            else
            {
                throw new Exception("ProductCategory no found");
            }
        }
    }
}
