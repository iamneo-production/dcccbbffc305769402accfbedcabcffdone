using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using dotnetmvcapp.Controllers;
using dotnetmvcapp.Models;
using dotnetmvcapp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using Moq;

namespace dotnetmvcapp.Tests
{
        [TestFixture]
    public class AdControllerTests
    {
        private Mock<IAdService> mockAdService;
        private AdController controller;
        [SetUp]
        public void Setup()
        {
            mockAdService = new Mock<IAdService>();
            controller = new AdController(mockAdService.Object);
        }

        [Test]
        public void AddAd_ValidData_SuccessfulAddition_RedirectsToIndex()
        {
            // Arrange
            var mockAdService = new Mock<IAdService>();
            mockAdService.Setup(service => service.AddAd(It.IsAny<Ad>())).Returns(true);
            var controller = new AdController(mockAdService.Object);
            var ad = new Ad(); // Provide valid Ad data

            // Act
            var result = controller.AddAd(ad) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
        [Test]
        public void AddAd_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var mockAdService = new Mock<IAdService>();
            var controller = new AdController(mockAdService.Object);
            Ad invalidAd = null; // Invalid Ad data

            // Act
            var result = controller.AddAd(invalidAd) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid Ad data", result.Value);
        }
        [Test]
        public void AddAd_FailedAddition_ReturnsViewWithModelError()
        {
            // Arrange
            var mockAdService = new Mock<IAdService>();
            mockAdService.Setup(service => service.AddAd(It.IsAny<Ad>())).Returns(false);
            var controller = new AdController(mockAdService.Object);
            var ad = new Ad(); // Provide valid Ad data

            // Act
            var result = controller.AddAd(ad) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
            // Check for expected model state error
            Assert.AreEqual("Failed to add the Ad. Please try again.", controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }


        [Test]
        public void AddAd_Post_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockAdService = new Mock<IAdService>();
            mockAdService.Setup(service => service.AddAd(It.IsAny<Ad>())).Returns(true);
            var controller = new AdController(mockAdService.Object);
            var ad = new Ad();

            // Act
            var result = controller.AddAd(ad) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public void AddAd_Post_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            var mockAdService = new Mock<IAdService>();
            var controller = new AdController(mockAdService.Object);
            controller.ModelState.AddModelError("error", "Error");
            var ad = new Ad();

            // Act
            var result = controller.AddAd(ad) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ad, result.Model);
        }

        [Test]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var mockAdService = new Mock<IAdService>();
            mockAdService.Setup(service => service.GetAllAds()).Returns(new List<Ad>());
            var controller = new AdController(mockAdService.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

        [Test]
        public void Delete_ValidId_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockAdService = new Mock<IAdService>();
            mockAdService.Setup(service => service.DeleteAd(1)).Returns(true);
            var controller = new AdController(mockAdService.Object);

            // Act
            var result = controller.Delete(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public void Delete_InvalidId_ReturnsViewResult()
        {
            // Arrange
            var mockAdService = new Mock<IAdService>();
            mockAdService.Setup(service => service.DeleteAd(1)).Returns(false);
            var controller = new AdController(mockAdService.Object);

            // Act
            var result = controller.Delete(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }
    }
}
