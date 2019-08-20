using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EFProductRepository : IProductRepository
    {
        private readonly EFDbContext _context = new EFDbContext();
        public IEnumerable<Product> Products => _context.Products;

        public void SaveProduct(Product product)
        {
            if (product.ProductId == 0)
            {
                _context.Products.Add(product);
            }
            else
            {
                var tempProduct = _context.Products.Find(product.ProductId);
                if (tempProduct == null)
                    return;

                tempProduct.Name = product.Name;
                tempProduct.Description = product.Description;
                tempProduct.Price = product.Price;
                tempProduct.Category = product.Category;
            }
            _context.SaveChanges();
        }
    }
}