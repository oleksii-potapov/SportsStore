using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;

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

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductId = 1, Name = "P1", Category="Cat1"}
            }.AsQueryable());

            Cart cart = new Cart();
            CartController cartController = new CartController(mock.Object, null);

            cartController.AddToCart(cart, 1, null);

            Assert.AreEqual(1, cart.Lines.Count());
            Assert.AreEqual(1, cart.Lines.ToArray()[0].Product.ProductId);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductId = 1, Name = "P1", Category = "Cat1"}
            }.AsQueryable());

            Cart cart = new Cart();
            var target = new CartController(mock.Object, null);

            RedirectToRouteResult result = target.AddToCart(cart, 1, "myUrl");

            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("myUrl", result.RouteValues["returnUrl"]);
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            var cart = new Cart();
            var target = new CartController(null, null);
            var result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();
            CartController target = new CartController(null, mock.Object);

            var result = target.Checkout(cart, shippingDetails);

            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);
            Assert.AreEqual("", result.ViewName);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController controller = new CartController(null, mock.Object);
            controller.ModelState.AddModelError("error", "error");

            var result = controller.Checkout(cart, new ShippingDetails());
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);
            Assert.AreEqual("", result.ViewName);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            var controller = new CartController(null, mock.Object);
            var result = controller.Checkout(cart, new ShippingDetails());
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once);
            Assert.AreEqual("Completed", result.ViewName);
            Assert.IsTrue(result.ViewData.ModelState.IsValid);
        }
    }
}