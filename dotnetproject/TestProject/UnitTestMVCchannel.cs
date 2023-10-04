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
    public class ChannelControllerTests
    {
        private Mock<IChannelService> mockChannelService;
        private ChannelController controller;
        [SetUp]
        public void Setup()
        {
            mockChannelService = new Mock<IChannelService>();
            controller = new ChannelController(mockChannelService.Object);
        }

        [Test]
        public void AddChannel_ValidData_SuccessfulAddition_RedirectsToIndex()
        {
            // Arrange
            var mockChannelService = new Mock<IChannelService>();
            mockChannelService.Setup(service => service.AddChannel(It.IsAny<Channel>())).Returns(true);
            var controller = new ChannelController(mockChannelService.Object);
            var channel = new Channel(); // Provide valid Channel data

            // Act
            var result = controller.AddChannel(channel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
        [Test]
        public void AddChannel_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var mockChannelService = new Mock<IChannelService>();
            var controller = new ChannelController(mockChannelService.Object);
            Channel invalidChannel = null; // Invalid Channel data

            // Act
            var result = controller.AddChannel(invalidChannel) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid Channel data", result.Value);
        }
        [Test]
        public void AddChannel_FailedAddition_ReturnsViewWithModelError()
        {
            // Arrange
            var mockChannelService = new Mock<IChannelService>();
            mockChannelService.Setup(service => service.AddChannel(It.IsAny<Channel>())).Returns(false);
            var controller = new ChannelController(mockChannelService.Object);
            var channel = new Channel(); // Provide valid Channel data

            // Act
            var result = controller.AddChannel(channel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
            // Check for expected model state error
            Assert.AreEqual("Failed to add the Channel. Please try again.", controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }


        [Test]
        public void AddChannel_Post_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockChannelService = new Mock<IChannelService>();
            mockChannelService.Setup(service => service.AddChannel(It.IsAny<Channel>())).Returns(true);
            var controller = new ChannelController(mockChannelService.Object);
            var channel = new Channel();

            // Act
            var result = controller.AddChannel(channel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public void AddChannel_Post_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            var mockChannelService = new Mock<IChannelService>();
            var controller = new ChannelController(mockChannelService.Object);
            controller.ModelState.AddModelError("error", "Error");
            var channel = new Channel();

            // Act
            var result = controller.AddChannel(channel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(channel, result.Model);
        }

        [Test]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var mockChannelService = new Mock<IChannelService>();
            mockChannelService.Setup(service => service.GetAllChannels()).Returns(new List<Channel>());
            var controller = new ChannelController(mockChannelService.Object);

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
            var mockChannelService = new Mock<IChannelService>();
            mockChannelService.Setup(service => service.DeleteChannel(1)).Returns(true);
            var controller = new ChannelController(mockChannelService.Object);

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
            var mockChannelService = new Mock<IChannelService>();
            mockChannelService.Setup(service => service.DeleteChannel(1)).Returns(false);
            var controller = new ChannelController(mockChannelService.Object);

            // Act
            var result = controller.Delete(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }
    }
}
