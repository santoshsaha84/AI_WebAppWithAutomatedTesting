using BulkyBook.Areas.Admin.Controllers;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BulkyBook.Tests
{
    public class CategoryControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICategoryRepository> _mockCategoryRepo;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCategoryRepo = new Mock<ICategoryRepository>();
            _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepo.Object);
            _controller = new CategoryController(_mockUnitOfWork.Object);
        }

        [Fact]
        public void Index_ReturnsAViewResult_WithAListOfCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Cat 1" },
                new Category { Id = 2, Name = "Cat 2" }
            };
            _mockCategoryRepo.Setup(u => u.GetAll(null)).Returns(categories);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Category>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Create_Post_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };

            // Act
            var result = _controller.Create(category);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _mockCategoryRepo.Verify(u => u.Add(It.IsAny<Category>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }
    }
}
