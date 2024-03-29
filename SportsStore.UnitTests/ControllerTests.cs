﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using System.Linq;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class ControllerTests
    {
        [TestMethod]
        public void Can_Paginate()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products)
                .Returns(new Product[] {
                    new Product {ProductId = 1, Name="P1"},
                    new Product {ProductId = 2, Name="P2"},
                    new Product {ProductId = 3, Name="P3"},
                    new Product {ProductId = 4, Name="P4"},
                    new Product {ProductId = 5, Name="P5"},
                });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            ProductsListViewModel result =
                (ProductsListViewModel)controller.List(null, 2).Model;

            // Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            // Arrange - define an HTML helper
            HtmlHelper helper = null;

            // Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // Arrange - set up delegate using lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            // Act
            MvcHtmlString result = helper.PageLinks(pagingInfo, pageUrlDelegate);

            //Assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>" +
                @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>" +
                @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name="P1"},
                new Product {ProductId = 2, Name="P2"},
                new Product {ProductId = 3, Name="P3"},
                new Product {ProductId = 4, Name="P4"},
                new Product {ProductId = 5, Name="P5"},
            });

            // Arrange
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            ProductsListViewModel model = (ProductsListViewModel)controller.List(null, 2).Model;

            // Assert
            PagingInfo pagingInfo = model.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name="P1", Category = "Cat1"},
                new Product {ProductId = 2, Name="P2", Category = "Cat2"},
                new Product {ProductId = 3, Name="P3", Category = "Cat1"},
                new Product {ProductId = 4, Name="P4", Category = "Cat2"},
                new Product {ProductId = 5, Name="P5", Category = "Cat1"},
            });

            //Arrange
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            ProductsListViewModel model = (ProductsListViewModel)controller.List("Cat1", 1).Model;

            //Assert
            Assert.AreEqual("P1", model.Products.ElementAt(0).Name);
            Assert.AreEqual("P3", model.Products.ElementAt(1).Name);
            Assert.AreEqual("P5", model.Products.ElementAt(2).Name);
            Assert.AreEqual(3, model.Products.Count());
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name="P1", Category = "Cat1"},
                new Product {ProductId = 5, Name="P5", Category = "Cat3"},
                new Product {ProductId = 2, Name="P2", Category = "Cat2"},
                new Product {ProductId = 3, Name="P3", Category = "Cat1"},
                new Product {ProductId = 4, Name="P4", Category = "Cat2"},
            });
            NavController controller = new NavController(mock.Object);

            // Act
            var categories = (IEnumerable<string>)controller.Menu().Model;

            // Assert
            Assert.AreEqual(3, categories.Count());
            Assert.AreEqual("Cat1", categories.ElementAt(0));
            Assert.AreEqual("Cat2", categories.ElementAt(1));
            Assert.AreEqual("Cat3", categories.ElementAt(2));
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductId = 1, Name = "P1", Category = "Apples"},
                new Product {ProductId = 4, Name = "P2", Category = "Oranges"},
            });
            NavController controller = new NavController(mock.Object);
            string categorySelected = "Apples";
            string result = controller.Menu(categorySelected).ViewBag.SelectedCategory;
            Assert.AreEqual(categorySelected, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name="P1", Category = "Cat1"},
                new Product {ProductId = 5, Name="P5", Category = "Cat3"},
                new Product {ProductId = 2, Name="P2", Category = "Cat2"},
                new Product {ProductId = 3, Name="P3", Category = "Cat1"},
                new Product {ProductId = 4, Name="P4", Category = "Cat2"},
            });
            ProductController controller = new ProductController(mock.Object);

            var cat1 = (ProductsListViewModel)controller.List(category: "Cat1").Model;
            var cat2 = (ProductsListViewModel)controller.List(category: "Cat2").Model;
            var cat3 = (ProductsListViewModel)controller.List(category: "Cat3").Model;
            var catAll = (ProductsListViewModel)controller.List(category: null).Model;

            Assert.AreEqual(2, cat1.PagingInfo.TotalItems);
            Assert.AreEqual(2, cat2.PagingInfo.TotalItems);
            Assert.AreEqual(1, cat3.PagingInfo.TotalItems);
            Assert.AreEqual(5, catAll.PagingInfo.TotalItems);
        }
    }
}