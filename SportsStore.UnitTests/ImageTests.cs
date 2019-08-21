using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using Moq;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        public void Can_Retrieve_Image_Data()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product
                {
                    ProductId = 2,
                    Name = "P2",
                    ImageMimeType = "image/png",
                    ImageData = new byte[] {}
                }
            });
            ProductController target = new ProductController(mock.Object);

            var result = target.GetImage(2);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual("image/png", ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Cannot_Retrieve_Image_Data_For_Invalid_Id()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product
                {
                    ProductId = 2,
                    Name = "P2",
                    ImageMimeType = "image/png",
                    ImageData = new byte[] {}
                }
            });
            ProductController target = new ProductController(mock.Object);

            var result = target.GetImage(1);

            Assert.IsNull(result);
        }
    }
}