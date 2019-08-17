using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Entities
{
    public class Cart
    {
        private readonly List<CartLine> _lineCollection = new List<CartLine>();

        public IEnumerable<CartLine> Lines => _lineCollection;

        public void AddItem(Product product, int quantity)
        {
            var line = _lineCollection.Find(p => p.Product.ProductId == product.ProductId);
            if (line == null)
            {
                _lineCollection.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(Product product)
        {
            _lineCollection.RemoveAll(c => c.Product.ProductId == product.ProductId);
        }

        public decimal ComputeTotalValue()
        {
            return _lineCollection.Sum(c => c.Product.Price * c.Quantity);
        }

        public void Clear()
        {
            _lineCollection.Clear();
        }
    }

    public class CartLine
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}