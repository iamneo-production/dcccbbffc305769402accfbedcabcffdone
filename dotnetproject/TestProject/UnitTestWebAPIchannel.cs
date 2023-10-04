using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using dotnetapiapp.Controllers;
using dotnetapiapp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
namespace dotnetapiapp.Tests
{
    [TestFixture]
    public class ChannelControllerTests
    {
        private ChannelController _ChannelController;
        private ChannelAdDbContext _context;

        [SetUp]
        public void Setup()
        {
            // Initialize an in-memory database for testing
            var options = new DbContextOptionsBuilder<ChannelAdDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ChannelAdDbContext(options);
            _context.Database.EnsureCreated(); // Create the database

            // Seed the database with sample data
            _context.Channels.AddRange(new List<Channel>
            {
                new Channel { ChannelID = 1, ChannelName = "Channel 1", POCName = "POC Name 1", CommercialPerAd = 1000,MailID="Channelmail1@gmaul.com",ContactNumber="1234567890" },
                new Channel { ChannelID = 2, ChannelName = "Channel 2", POCName = "POC Name 2", CommercialPerAd = 2000,MailID="Channelmail2@gmaul.com",ContactNumber="9876543210" },
                new Channel { ChannelID = 3, ChannelName = "Channel 3", POCName = "POC Name 3", CommercialPerAd = 3000,MailID="Channelmail3@gmaul.com",ContactNumber="9876543212" }
            });
            _context.SaveChanges();

            _ChannelController = new ChannelController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted(); // Delete the in-memory database after each test
            _context.Dispose();
        }
        [Test]
        public void ChannelClassExists()
        {
            // Arrange
            Type ChannelType = typeof(Channel);

            // Act & Assert
            Assert.IsNotNull(ChannelType, "Channel class not found.");
        }
        [Test]
        public void Channel_Properties_ChannelName_ReturnExpectedDataTypes()
        {
            // Arrange
            Channel channel = new Channel();
            PropertyInfo propertyInfo = channel.GetType().GetProperty("ChannelName");
            // Act & Assert
            Assert.IsNotNull(propertyInfo, "ChannelName property not found.");
            Assert.AreEqual(typeof(string), propertyInfo.PropertyType, "ChannelName property type is not string.");
        }
[Test]
        public void Channel_Properties_POCName_ReturnExpectedDataTypes()
        {
            // Arrange
            Channel channel = new Channel();
            PropertyInfo propertyInfo = channel.GetType().GetProperty("POCName");
            // Act & Assert
            Assert.IsNotNull(propertyInfo, "POCName property not found.");
            Assert.AreEqual(typeof(string), propertyInfo.PropertyType, "POCName property type is not string.");
        }

        [Test]
        public async Task GetAllChannels_ReturnsOkResult()
        {
            // Act
            var result = await _ChannelController.GetAllChannels();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetAllChannels_ReturnsAllChannels()
        {
            // Act
            var result = await _ChannelController.GetAllChannels();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;

            Assert.IsInstanceOf<IEnumerable<Channel>>(okResult.Value);
            var channels = okResult.Value as IEnumerable<Channel>;

            var ChannelCount = channels.Count();
            Assert.AreEqual(3, ChannelCount); // Assuming you have 3 Channels in the seeded data
        }


        [Test]
        public async Task AddChannel_ValidData_ReturnsOkResult()
        {
            // Arrange
            var newChannel = new Channel
            {
 ChannelName = "Channel New", POCName = "New POC Name", CommercialPerAd = 4000,MailID="Channelmailnew@gmaul.com",ContactNumber="9934567890"
            };

            // Act
            var result = await _ChannelController.AddChannel(newChannel);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }
        [Test]
        public async Task DeleteChannel_ValidId_ReturnsNoContent()
        {
            // Arrange
              // var controller = new ChannelsController(context);

                // Act
                var result = await _ChannelController.DeleteChannel(1) as NoContentResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public async Task DeleteChannel_InvalidId_ReturnsBadRequest()
        {
                   // Act
                var result = await _ChannelController.DeleteChannel(0) as BadRequestObjectResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(400, result.StatusCode);
                Assert.AreEqual("Not a valid Channel id", result.Value);
        }
    }
}
