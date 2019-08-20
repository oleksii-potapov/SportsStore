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
    }
}