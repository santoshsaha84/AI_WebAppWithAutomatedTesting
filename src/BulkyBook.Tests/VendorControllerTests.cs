using BulkyBook.Areas.Admin.Controllers;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace BulkyBook.Tests
{
    public class VendorControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IVendorRepository> _mockVendorRepo;
        private readonly VendorController _controller;

        public VendorControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockVendorRepo = new Mock<IVendorRepository>();
            _mockUnitOfWork.Setup(u => u.Vendor).Returns(_mockVendorRepo.Object);
            _controller = new VendorController(_mockUnitOfWork.Object);
            _controller.TempData = new Mock<ITempDataDictionary>().Object;
        }

        [Fact]
        public void Index_ReturnsAViewResult_WithAListOfVendors()
        {
            // Arrange
            var vendors = new List<Vendor>
            {
                new Vendor { Id = 1, Name = "Vendor 1", City = "City 1", Address = "Addr 1" },
                new Vendor { Id = 2, Name = "Vendor 2", City = "City 2", Address = "Addr 2" }
            };
            _mockVendorRepo.Setup(u => u.GetAll(null)).Returns(vendors);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Vendor>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Create_Post_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var vendor = new Vendor { Name = "Test Vendor", City = "Test City", Address = "Test Addr" };

            // Act
            var result = _controller.Create(vendor);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _mockVendorRepo.Verify(u => u.Add(It.IsAny<Vendor>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void Edit_Post_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var vendor = new Vendor { Id = 1, Name = "Updated Vendor", City = "City", Address = "Addr" };

            // Act
            var result = _controller.Edit(vendor);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _mockVendorRepo.Verify(u => u.Update(It.IsAny<Vendor>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void DeletePost_RedirectsToIndex_WhenVendorExists()
        {
            // Arrange
            var vendor = new Vendor { Id = 1, Name = "To Delete" };
            _mockVendorRepo.Setup(u => u.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Vendor, bool>>>(), null)).Returns(vendor);

            // Act
            var result = _controller.DeletePost(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _mockVendorRepo.Verify(u => u.Remove(vendor), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }
    }
}
