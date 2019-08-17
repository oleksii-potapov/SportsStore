using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            Product p1 = new Product { Name = "P1", ProductId = 1 };
            Product p2 = new Product { Name = "P2", ProductId = 2 };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            var result = target.Lines.ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(p1, result[0].Product);
            Assert.AreEqual(p2, result[1].Product);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            Product p1 = new Product { Name = "P1", ProductId = 1 };
            Cart target = new Cart();
            target.AddItem(p1, 1);
            target.AddItem(p1, 2);
            var result = target.Lines.ToArray();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(3, result[0].Quantity);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            Product p1 = new Product { Name = "P1", ProductId = 1 };
            Product p2 = new Product { Name = "P2", ProductId = 2 };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.RemoveLine(p1);
            var result = target.Lines.ToArray();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(p2, result[0].Product);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            var p1 = new Product { ProductId = 1, Price = 10 };
            var p2 = new Product { ProductId = 2, Price = 15 };
            var p3 = new Product { ProductId = 3, Price = 20 };
            Cart cart = new Cart();
            cart.AddItem(p1, 3);
            cart.AddItem(p2, 2);
            cart.AddItem(p3, 1);
            Assert.AreEqual(80M, cart.ComputeTotalValue());
        }

        [TestMethod]
        public void Can_Clear_Content()
        {
            var p1 = new Product { ProductId = 1, Price = 10 };
            var p2 = new Product { ProductId = 2, Price = 15 };
            var p3 = new Product { ProductId = 3, Price = 20 };
            Cart cart = new Cart();
            cart.AddItem(p1, 3);
            cart.AddItem(p2, 2);
            cart.AddItem(p3, 1);
            cart.Clear();
            Assert.AreEqual(0, cart.Lines.Count());
        }
    }
}