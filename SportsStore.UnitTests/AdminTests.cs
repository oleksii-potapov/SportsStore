using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductId = 1, Name="P1"},
                new Product {ProductId = 2, Name="P2"},
                new Product {ProductId = 3, Name="P3"},
            });
            AdminController target = new AdminController(mock.Object);
            var result = (IEnumerable<Product>)target.Index().Model;

            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("P3", result.ElementAt(2).Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductId = 1, Name="P1"},
                new Product {ProductId = 2, Name="P2"},
                new Product {ProductId = 3, Name="P3"},
            });
            AdminController target = new AdminController(mock.Object);
            var result = (Product)target.Edit(2).Model;

            Assert.AreEqual("P2", result.Name);
        }

        [TestMethod]
        public void Can_Edit_NonExist_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductId = 1, Name="P1"},
                new Product {ProductId = 2, Name="P2"},
                new Product {ProductId = 3, Name="P3"},
            });
            AdminController target = new AdminController(mock.Object);
            var result = (Product)target.Edit(4).Model;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductId = 1, Name="P1"},
            });
            AdminController target = new AdminController(mock.Object);
            var model = (Product)target.Edit(1).Model;
            model.Name = "Success";
            var result = target.Edit(model);

            mock.Verify(m => m.SaveProduct(model));
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };
            target.ModelState.AddModelError("error", "error");

            var result = target.Edit(product);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}