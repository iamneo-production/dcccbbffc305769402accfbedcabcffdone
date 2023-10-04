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
    public class AdControllerTests
    {
        private AdController _AdController;
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
            _context.Ads.AddRange(new List<Ad>
            {
new Ad { AdID = 1, BrandName = "Ad Brand New", NumberOfTimes = 20, BroadcastDate = DateTime.Parse("2023-08-30"),Duration=22,Description ="one description", ChannelName="ChannelName1" },
new Ad { AdID = 2, BrandName = "Ad Brand New", NumberOfTimes = 27, BroadcastDate = DateTime.Parse("2023-08-20"),Duration=20,Description ="two description", ChannelName="ChannelName2" },
new Ad { AdID = 3, BrandName = "Ad Brand New", NumberOfTimes = 28, BroadcastDate = DateTime.Parse("2023-08-10"),Duration=28,Description ="three description", ChannelName="ChannelName3" }
            });
            _context.SaveChanges();

            _AdController = new AdController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted(); // Delete the in-memory database after each test
            _context.Dispose();
        }
        [Test]
        public void AdClassExists()
        {
            // Arrange
            Type AdType = typeof(Ad);

            // Act & Assert
            Assert.IsNotNull(AdType, "Ad class not found.");
        }
        [Test]
        public void Ad_Properties_BrandName_ReturnExpectedDataTypes()
        {
            // Arrange
            Ad ad = new Ad();
            PropertyInfo propertyInfo = ad.GetType().GetProperty("BrandName");
            // Act & Assert
            Assert.IsNotNull(propertyInfo, "BrandName property not found.");
            Assert.AreEqual(typeof(string), propertyInfo.PropertyType, "BrandName property type is not string.");
        }
[Test]
        public void Ad_Properties_ChannelName_ReturnExpectedDataTypes()
        {
            // Arrange
            Ad ad = new Ad();
            PropertyInfo propertyInfo = ad.GetType().GetProperty("ChannelName");
            // Act & Assert
            Assert.IsNotNull(propertyInfo, "ChannelName property not found.");
            Assert.AreEqual(typeof(string), propertyInfo.PropertyType, "ChannelName property type is not string.");
        }

        [Test]
        public async Task GetAllAds_ReturnsOkResult()
        {
            // Act
            var result = await _AdController.GetAllAds();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetAllAds_ReturnsAllAds()
        {
            // Act
            var result = await _AdController.GetAllAds();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;

            Assert.IsInstanceOf<IEnumerable<Ad>>(okResult.Value);
            var ads = okResult.Value as IEnumerable<Ad>;

            var AdCount = ads.Count();
            Assert.AreEqual(3, AdCount); // Assuming you have 3 Ads in the seeded data
        }


        [Test]
        public async Task AddAd_ValidData_ReturnsOkResult()
        {
            // Arrange
            var newAd = new Ad
            {
BrandName = "Ad Brand New", NumberOfTimes = 7, BroadcastDate = DateTime.Parse("2023-08-30"),Duration=22,Description ="New description", ChannelName="Channel New"
            };

            // Act
            var result = await _AdController.AddAd(newAd);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }
        [Test]
        public async Task DeleteAd_ValidId_ReturnsNoContent()
        {
            // Arrange
              // var controller = new AdsController(context);

                // Act
                var result = await _AdController.DeleteAd(1) as NoContentResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public async Task DeleteAd_InvalidId_ReturnsBadRequest()
        {
                   // Act
                var result = await _AdController.DeleteAd(0) as BadRequestObjectResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(400, result.StatusCode);
                Assert.AreEqual("Not a valid Ad id", result.Value);
        }
    }
}
